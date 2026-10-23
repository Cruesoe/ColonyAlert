# Draws the alert banner background and warning icon into Textures\ColonyAlert, in the preview's red, at twice their on-screen size.
param(
    [string]$OutDir = (Join-Path $PSScriptRoot '..\Textures\ColonyAlert'),
    [int]$Red = 214,
    [int]$Green = 64,
    [int]$Blue = 58
)
Add-Type -AssemblyName System.Drawing
New-Item -ItemType Directory -Force -Path $OutDir | Out-Null
$OutDir = (Resolve-Path $OutDir).Path
$accent = [System.Drawing.Color]::FromArgb($Red, $Green, $Blue)

function New-RoundedPath($x, $y, $w, $h, $r) {
    $p = New-Object System.Drawing.Drawing2D.GraphicsPath
    $p.AddArc($x, $y, 2 * $r, 2 * $r, 180, 90)
    $p.AddArc($x + $w - 2 * $r, $y, 2 * $r, 2 * $r, 270, 90)
    $p.AddArc($x + $w - 2 * $r, $y + $h - 2 * $r, 2 * $r, 2 * $r, 0, 90)
    $p.AddArc($x, $y + $h - 2 * $r, 2 * $r, 2 * $r, 90, 90)
    $p.CloseFigure()
    $p
}

# Banner: 440x88 for a 220x44 rect. Dark plate with a red sheen, hazard stripes at both ends and a red outline.
$w = 440; $h = 88; $stripe = 48
$bmp = New-Object System.Drawing.Bitmap $w, $h
$g = [System.Drawing.Graphics]::FromImage($bmp)
$g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
$g.Clear([System.Drawing.Color]::Transparent)
$plate = New-RoundedPath 2 2 ($w - 4) ($h - 4) 10
$g.SetClip($plate)
$sheen = New-Object System.Drawing.Drawing2D.LinearGradientBrush (New-Object System.Drawing.Point 0, 0), (New-Object System.Drawing.Point 0, $h), ([System.Drawing.Color]::FromArgb(245, 74, 24, 24)), ([System.Drawing.Color]::FromArgb(245, 24, 20, 22))
$g.FillRectangle($sheen, 0, 0, $w, $h)
$dark = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(250, 20, 20, 22))
$accentBrush = New-Object System.Drawing.SolidBrush $accent
foreach ($x0 in 0, ($w - $stripe)) {
    $g.FillRectangle($dark, $x0, 0, $stripe, $h)
    $old = $g.Clip
    $g.SetClip((New-Object System.Drawing.Rectangle $x0, 0, $stripe, $h), [System.Drawing.Drawing2D.CombineMode]::Intersect)
    for ($s = -$h; $s -lt $stripe + $h; $s += 28) {
        $pts = [System.Drawing.PointF[]]@(
            (New-Object System.Drawing.PointF ($x0 + $s), $h),
            (New-Object System.Drawing.PointF ($x0 + $s + 14), $h),
            (New-Object System.Drawing.PointF ($x0 + $s + 14 + $h), 0),
            (New-Object System.Drawing.PointF ($x0 + $s + $h), 0))
        $g.FillPolygon($accentBrush, $pts)
    }
    $g.Clip = $old
}
# Dividers between stripes and plate, and a thin highlight along the top
$g.FillRectangle($accentBrush, $stripe, 0, 3, $h)
$g.FillRectangle($accentBrush, $w - $stripe - 3, 0, 3, $h)
$g.FillRectangle((New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(60, 255, 255, 255))), $stripe + 3, 6, $w - 2 * $stripe - 6, 2)
$g.ResetClip()
$g.DrawPath((New-Object System.Drawing.Pen $accent, 4), $plate)
$g.Dispose()
$bmp.Save((Join-Path $OutDir 'Banner.png'), [System.Drawing.Imaging.ImageFormat]::Png)
$bmp.Dispose()

# Warning icon: 64x64 for a 32x32 rect. Red triangle with a light rim and a white exclamation mark.
$bmp = New-Object System.Drawing.Bitmap 64, 64
$g = [System.Drawing.Graphics]::FromImage($bmp)
$g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
$g.Clear([System.Drawing.Color]::Transparent)
$tri = New-Object System.Drawing.Drawing2D.GraphicsPath
$tri.AddPolygon([System.Drawing.PointF[]]@((New-Object System.Drawing.PointF 32, 5), (New-Object System.Drawing.PointF 60, 57), (New-Object System.Drawing.PointF 4, 57)))
$g.FillPath($accentBrush, $tri)
$rim = New-Object System.Drawing.Pen ([System.Drawing.Color]::FromArgb(255, 255, 236, 230)), 4
$rim.LineJoin = [System.Drawing.Drawing2D.LineJoin]::Round
$g.DrawPath($rim, $tri)
$white = [System.Drawing.Brushes]::White
$g.FillPath($white, (New-RoundedPath 28.5 21 7 22 3.5))
$g.FillEllipse($white, 28, 46, 8, 8)
$g.Dispose()
$bmp.Save((Join-Path $OutDir 'Warning.png'), [System.Drawing.Imaging.ImageFormat]::Png)
$bmp.Dispose()
