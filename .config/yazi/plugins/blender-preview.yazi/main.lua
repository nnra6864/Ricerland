local M = {}

function M:peek(job)
	local start, cache = os.clock(), ya.file_cache(job)
	if not cache then
		return
	end

	local ok, err = self:preload(job)
	if not ok or err then
		return ya.preview_widget(job, err)
	end

	ya.sleep(math.max(0, rt.preview.image_delay / 1000 + start - os.clock()))

	local _, err = ya.image_show(cache, job.area)
	ya.preview_widget(job, err)
end

function M:seek() end

function M:preload(job)
	local cache = ya.file_cache(job)
	if not cache or fs.cha(cache) then
		return true
	end

	local cmd = Command("blender-thumbnailer"):arg({ tostring(job.file.url), tostring(cache) })

	local status, err = cmd:status()
	if not status then
		return true, Err("Failed to start `blender-thumbnailer`, error: %s", err)
	elseif not status.success then
		return false, Err("`blender-thumbnailer` exited with error code: %s", status.code)
	else
		return true
	end
end

function M:spot(job)
	require("file"):spot(job)
end

return M
