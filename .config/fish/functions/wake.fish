function wake --argument-names device
    set -l mac_nous "b4:2e:99:a2:03:9e"
    set -l mac_trowe "70:85:c2:a4:d4:aa"

    switch $device
        case "nous"
            wol $mac_nous
        case "trowe"
            wol $mac_trowe
        case ""
            echo "Error: Device name required (nous or trowe)." >&2
            return 1
        case "*"
            echo "Error: Unknown device '$device'. Allowed options: nous, trowe" >&2
            return 1
    end
end
