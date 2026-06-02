local defs = require("defs")

hl.config {
    plugin = {
        dynamic_cursors = {
            enabled   = true,
            mode      = "rotate",
            threshold = 1,

            rotate = {
                length = defs.cursor.size,
                offset = 21
            },

            shake = {
                enabled = false
            },

            hyprcursor = {
                enabled = true
            }
        }
    }
}
