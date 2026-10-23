# Draws About\Preview.png in the style of the Research mods' previews, in alert red, with the two statuses as cards and a faint screen border.
param(
    [string]$Out = (Join-Path $PSScriptRoot '..\About\Preview.png'),
    [int]$Red = 214,
    [int]$Green = 64,
    [int]$Blue = 58
)
Add-Type -AssemblyName System.Drawing

$bmp = New-Object System.Drawing.Bitmap 1280, 720
$g = [System.Drawing.Graphics]::FromImage($bmp)
$g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
$g.Clear([System.Drawing.Color]::FromArgb(21, 24, 29))
$accent = [System.Drawing.Color]::FromArgb($Red, $Green, $Blue)

# The in-game alert border
$g.DrawRectangle((New-Object System.Drawing.Pen ([System.Drawing.Color]::FromArgb(150, $Red, $Green, $Blue)), 8), 4, 4, 1272, 712)

# Draws text as a path scaled so its ink is exactly the given cap height, top-left at (x, y), with extra letter spacing.
function Draw-Text($text, $family, $style, $x, $y, $capHeight, $spacing, $brush) {
    $fam = New-Object System.Drawing.FontFamily $family
    $probe = New-Object System.Drawing.Drawing2D.GraphicsPath
    $probe.AddString('H', $fam, [int]$style, 100, (New-Object System.Drawing.PointF 0, 0), [System.Drawing.StringFormat]::GenericTypographic)
    $scale = $capHeight / $probe.GetBounds().Height
    $cursor = 0.0
    $path = New-Object System.Drawing.Drawing2D.GraphicsPath
    $path.FillMode = [System.Drawing.Drawing2D.FillMode]::Winding
    if ($spacing -eq 0) { $path.AddString($text, $fam, [int]$style, 100 * $scale, (New-Object System.Drawing.PointF 0, 0), [System.Drawing.StringFormat]::GenericTypographic) } else {
    foreach ($ch in $text.ToCharArray()) {
        $glyph = New-Object System.Drawing.Drawing2D.GraphicsPath
        $glyph.AddString([string]$ch, $fam, [int]$style, 100 * $scale, (New-Object System.Drawing.PointF $cursor, 0), [System.Drawing.StringFormat]::GenericTypographic)
        $path.AddPath($glyph, $false)
        $size = $g.MeasureString([string]$ch, (New-Object System.Drawing.Font $fam, ([single](100 * $scale)), ([System.Drawing.FontStyle]$style), ([System.Drawing.GraphicsUnit]::Pixel)), [System.Drawing.PointF]::Empty, [System.Drawing.StringFormat]::GenericTypographic)
        $cursor += $size.Width + $spacing
    }
    }
    $bounds = $path.GetBounds()
    $m = New-Object System.Drawing.Drawing2D.Matrix
    $m.Translate($x - $bounds.X, $y - $bounds.Y)
    $path.Transform($m)
    $g.FillPath($brush, $path)
}

# Bar, label and word at the Research previews' positions
$g.FillRectangle((New-Object System.Drawing.SolidBrush $accent), 100, 189, 140, 10)
Draw-Text 'COLONY' 'Bahnschrift' 0 107 238 59 22 (New-Object System.Drawing.SolidBrush $accent)
Draw-Text 'ALERT!' 'Bahnschrift SemiBold' 0 98 358 178 0 ([System.Drawing.Brushes]::White)

# Two status cards on the right: normal faint, alert highlighted in the accent like the active one
$dim = [System.Drawing.Color]::FromArgb(80, 255, 255, 255)
$g.FillRectangle((New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(22, 255, 255, 255))), 960, 238, 250, 100)
$g.DrawRectangle((New-Object System.Drawing.Pen $dim, 2), 960, 238, 250, 100)
$g.DrawEllipse((New-Object System.Drawing.Pen $dim, 3), 984, 272, 32, 32)
Draw-Text 'NORMAL' 'Bahnschrift' 0 1030 278 22 5 (New-Object System.Drawing.SolidBrush $dim)

$g.FillRectangle((New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(70, $Red, $Green, $Blue))), 960, 436, 250, 100)
$g.DrawRectangle((New-Object System.Drawing.Pen ([System.Drawing.Color]::FromArgb(220, $Red, $Green, $Blue)), 2), 960, 436, 250, 100)
$g.FillEllipse((New-Object System.Drawing.SolidBrush $accent), 984, 470, 32, 32)
Draw-Text 'ALERT' 'Bahnschrift SemiBold' 0 1030 476 22 5 ([System.Drawing.Brushes]::White)

# Arrows between the cards for the toggle
$arrow = New-Object System.Drawing.Pen ([System.Drawing.Color]::FromArgb(140, 255, 255, 255)), 3
$arrow.CustomEndCap = New-Object System.Drawing.Drawing2D.AdjustableArrowCap 4, 4
$g.DrawLine($arrow, 1065, 352, 1065, 422)
$g.DrawLine($arrow, 1105, 422, 1105, 352)

$bmp.Save([IO.Path]::GetFullPath($Out), [System.Drawing.Imaging.ImageFormat]::Png)
$g.Dispose(); $bmp.Dispose()
