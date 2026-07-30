require("full-border"):setup()

-- Update zoxide on CWD
require("zoxide"):setup {
	update_db = true,
}

require("git"):setup {
	-- Order of status signs showing in the linemode
	order = 1500,
}
