# Synthesizes the switching sounds into Sounds\Alert as 16-bit mono 44.1 kHz WAV: a soft two-blast klaxon for going on alert and a falling two-tone chime for standing down.
param(
    [string]$OutDir = (Join-Path $PSScriptRoot '..\Sounds\Alert')
)
New-Item -ItemType Directory -Force -Path $OutDir | Out-Null
$OutDir = (Resolve-Path $OutDir).Path
$rate = 44100

function Save-Wav([string]$path, [double[]]$samples) {
    $stream = [System.IO.File]::Create($path)
    $w = New-Object System.IO.BinaryWriter $stream
    $dataBytes = $samples.Length * 2
    $w.Write([Text.Encoding]::ASCII.GetBytes('RIFF')); $w.Write([int](36 + $dataBytes)); $w.Write([Text.Encoding]::ASCII.GetBytes('WAVE'))
    $w.Write([Text.Encoding]::ASCII.GetBytes('fmt ')); $w.Write([int]16); $w.Write([int16]1); $w.Write([int16]1)
    $w.Write([int]$rate); $w.Write([int]($rate * 2)); $w.Write([int16]2); $w.Write([int16]16)
    $w.Write([Text.Encoding]::ASCII.GetBytes('data')); $w.Write([int]$dataBytes)
    foreach ($s in $samples) { $w.Write([int16][Math]::Round([Math]::Max(-1.0, [Math]::Min(1.0, $s)) * 32767)) }
    $w.Close()
}

# Klaxon: two short blasts, each sweeping up from 360 to 440 Hz, mostly sine with a little saw, softened by a one-pole low-pass.
$blast = 0.3; $gap = 0.12
$n = [int](($blast * 2 + $gap + 0.05) * $rate)
$k = New-Object double[] $n
$phase = 0.0; $lp = 0.0
for ($i = 0; $i -lt $n; $i++) {
    $t = $i / $rate
    $local = if ($t -lt $blast) { $t } elseif ($t -ge $blast + $gap -and $t -lt 2 * $blast + $gap) { $t - $blast - $gap } else { -1 }
    if ($local -lt 0) { $lp *= 0.9; $k[$i] = $lp; continue }
    $f = 360 + 80 * [Math]::Min(1.0, $local / 0.1)
    $phase = ($phase + $f / $rate) % 1.0
    $raw = 0.75 * [Math]::Sin(2 * [Math]::PI * $phase) + 0.25 * (2 * $phase - 1)
    $lp += 0.15 * ($raw - $lp)
    $env = [Math]::Min(1.0, $local / 0.025) * [Math]::Min(1.0, ($blast - $local) / 0.08)
    $k[$i] = 0.9 * $lp * $env
}
Save-Wav (Join-Path $OutDir 'Klaxon.wav') $k

# Stand down: 880 Hz then 660 Hz bell tones with a quieter octave partial and exponential decay.
$n = [int](1.1 * $rate)
$c = New-Object double[] $n
foreach ($note in @(@{ Start = 0.0; Freq = 880.0 }, @{ Start = 0.2; Freq = 660.0 })) {
    $s0 = [int]($note.Start * $rate)
    for ($i = $s0; $i -lt $n; $i++) {
        $t = ($i - $s0) / $rate
        $env = [Math]::Min(1.0, $t / 0.01) * [Math]::Exp(-$t * 6.0)
        $c[$i] += 0.3 * $env * ([Math]::Sin(2 * [Math]::PI * $note.Freq * $t) + 0.1 * [Math]::Sin(4 * [Math]::PI * $note.Freq * $t))
    }
}
Save-Wav (Join-Path $OutDir 'StandDown.wav') $c
