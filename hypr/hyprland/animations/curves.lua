hl.curve("expo_out", {
    type = "bezier",
    points = {{0.16, 1}, {0.3, 1}}
})

hl.curve("spring", {
    type = "spring",
    mass = 1,
    stiffness = 70,
    dampening = 10
})
