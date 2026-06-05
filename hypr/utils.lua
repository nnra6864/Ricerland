local M = {}

-- Copies values to a new table
function M.copy(original)
    if type(original) ~= 'table' then return original end

    local duplicate = {}
    for key, value in pairs(original) do
        duplicate[key] = M.copy(value)
    end

    return duplicate
end

-- Recursively merges source table into target table
function M.deep_merge(target, source)
    for key, value in pairs(source) do
        if type(value) == "table" then
            if type(target[key]) ~= "table" then
                target[key] = {}
            end
            M.deep_merge(target[key], value)
        else
            target[key] = value
        end
    end
end

return M
