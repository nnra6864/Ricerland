// smooth_cursor.glsl
// The cursor stretches from its old cell to its new one: the leading edge snaps
// ahead fast, the trailing edge catches up a bit later, so it reads as one block
// gliding rather than a separate trail.
//
// Color: uses the terminal's cursor color (iCurrentCursorColor).
// Blending: Ghostty's framebuffer is premultiplied alpha, so the block is drawn
// opaque and composited "over" the frame. Keeping the background's alpha instead
// (as some shaders do) lets the wallpaper add onto the color when
// background-opacity < 1, making it look brighter than the real cursor.

// --- CONFIG ---
const float DURATION = 0.15; // seconds until the trailing edge arrives
const float LEAD     = 0.30; // leading edge's duration, as a fraction of DURATION
const float AA       = 1.0;  // antialiasing width in pixels

vec3 srgbToLinear(vec3 c) {
    return mix(c / 12.92, pow((c + 0.055) / 1.055, vec3(2.4)), step(vec3(0.04045), c));
}

float ease(float x) { // ease-out cubic
    float k = 1.0 - x;
    return 1.0 - k * k * k;
}

// Signed distance to a (possibly non-convex) quad, after Inigo Quilez.
float sdQuad(vec2 p, vec2 v0, vec2 v1, vec2 v2, vec2 v3) {
    vec2 v[4] = vec2[4](v0, v1, v2, v3);
    float d = dot(p - v[0], p - v[0]);
    float s = 1.0;
    for (int i = 0, j = 3; i < 4; j = i, i++) {
        vec2 e = v[j] - v[i];
        vec2 w = p - v[i];
        vec2 b = w - e * clamp(dot(w, e) / max(dot(e, e), 1e-6), 0.0, 1.0);
        d = min(d, dot(b, b));
        bvec3 c = bvec3(p.y >= v[i].y, p.y < v[j].y, e.x * w.y > e.y * w.x);
        if (all(c) || all(not(c))) s = -s;
    }
    return s * sqrt(d);
}

void mainImage(out vec4 fragColor, in vec2 fragCoord) {
    vec4 frame = texture(iChannel0, fragCoord / iResolution.xy);
    fragColor = frame;

    // iCurrentCursor.xy is the top-left corner (y up), .zw is width/height.
    vec2 halfC = iCurrentCursor.zw * 0.5;
    vec2 halfP = iPreviousCursor.zw * 0.5;
    vec2 centerC = iCurrentCursor.xy + vec2(halfC.x, -halfC.y);
    vec2 centerP = iPreviousCursor.xy + vec2(halfP.x, -halfP.y);

    vec2 move = centerC - centerP;
    float elapsed = iTime - iTimeCursorChange;
    if (dot(move, move) < 1.0 || elapsed >= DURATION) return;

    vec2 dir = normalize(move);

    // Corner order tl, tr, br, bl. Each corner's speed depends on how far it
    // points along the move: leading corners take DURATION*LEAD, trailing DURATION.
    vec2 o[4] = vec2[4](vec2(-1, 1), vec2(1, 1), vec2(1, -1), vec2(-1, -1));
    vec2 q[4];
    for (int i = 0; i < 4; i++) {
        float lead = clamp(dot(o[i], dir) * 0.70710678 * 0.5 + 0.5, 0.0, 1.0);
        float dur = DURATION * mix(1.0, LEAD, lead);
        float t = ease(clamp(elapsed / dur, 0.0, 1.0));
        q[i] = mix(centerP + o[i] * halfP, centerC + o[i] * halfC, t);
    }

    float d = sdQuad(fragCoord, q[0], q[1], q[2], q[3]);
    float cover = clamp(0.5 - d / AA, 0.0, 1.0);

    // Leave the real cursor cell alone so the glyph under it stays readable.
    vec2 inC = abs(fragCoord - centerC) - halfC;
    if (max(inC.x, inC.y) <= 0.0) cover = 0.0;

    vec3 color = srgbToLinear(iCurrentCursorColor.rgb);
    fragColor = vec4(color, 1.0) * cover + frame * (1.0 - cover);
}
