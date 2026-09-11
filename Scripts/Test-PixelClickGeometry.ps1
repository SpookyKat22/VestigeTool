param([string]$AssemblyPath)
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
$assembly = [Reflection.Assembly]::LoadFrom($AssemblyPath)
$pixelType = $assembly.GetType('_4RTools.Model.PixelMacro', $true)
$method = $pixelType.GetMethod('FindConnectedRegionCenter', [Reflection.BindingFlags]'NonPublic,Static')
$solid = New-Object byte[] 49
for ($y = 1; $y -le 5; $y++) { for ($x = 1; $x -le 5; $x++) { $solid[$y * 7 + $x] = 1 } }
$point = $method.Invoke($null, [object[]]@([byte[]]$solid, 7, 7, 1, 1))
if ($point.X -ne 3 -or $point.Y -ne 3) { throw 'Solid target center failed' }
$ring = New-Object byte[] 49
for ($i = 1; $i -le 5; $i++) {
    $ring[7 + $i] = 1; $ring[35 + $i] = 1
    $ring[$i * 7 + 1] = 1; $ring[$i * 7 + 5] = 1
}
$point = $method.Invoke($null, [object[]]@([byte[]]$ring, 7, 7, 1, 1))
if ($ring[$point.Y * 7 + $point.X] -ne 1) { throw 'Hollow target selected empty space' }
$interopType = $assembly.GetType('_4RTools.Utils.Interop', $true)
$inputType = $interopType.GetNestedType('INPUT', [Reflection.BindingFlags]'NonPublic')
$size = [Runtime.InteropServices.Marshal]::SizeOf([Activator]::CreateInstance($inputType))
$expected = if ([IntPtr]::Size -eq 8) { 40 } else { 28 }
if ($size -ne $expected) { throw "Invalid native INPUT size: $size" }
Write-Output 'PASS: solid target center, hollow target clicks matching color, native mouse input structure size.'

$scanMethod = $pixelType.GetMethod('TryFindBlockFromCenter', [Reflection.BindingFlags]'NonPublic,Static')
function Find-TestBlock([int]$width, [int]$height, [int]$blockSize, [byte[]]$map) {
    $integral = New-Object int[] (($width + 1) * ($height + 1))
    $stride = $width + 1
    for ($y = 0; $y -lt $height; $y++) {
        for ($x = 0; $x -lt $width; $x++) {
            $integral[($y + 1) * $stride + $x + 1] = $map[$y * $width + $x] + $integral[$y * $stride + $x + 1] + $integral[($y + 1) * $stride + $x] - $integral[$y * $stride + $x]
        }
    }
    $arguments = [object[]]@([byte[]]$map, [int[]]$integral, $width, $height, $blockSize, 0, 0)
    $found = $scanMethod.Invoke($null, $arguments)
    return @($found, $arguments[5], $arguments[6])
}
$map = New-Object byte[] 81
$map[4 * 9] = 1
$map[3 * 9 + 4] = 1
$result = Find-TestBlock 9 9 1 $map
if (!$result[0] -or $result[1] -ne 4 -or $result[2] -ne 3) { throw 'Scan preferred distant center-row target over nearby target' }
$map[4 * 9 + 4] = 1
$result = Find-TestBlock 9 9 1 $map
if ($result[1] -ne 4 -or $result[2] -ne 4) { throw 'Scan did not start at center' }
foreach ($shape in @(@(8, 5), @(1, 9), @(9, 1), @(1, 1))) {
    $width = $shape[0]; $height = $shape[1]
    for ($target = 0; $target -lt $width * $height; $target++) {
        $map = New-Object byte[] ($width * $height)
        $map[$target] = 1
        $result = Find-TestBlock $width $height 1 $map
        if (!$result[0] -or $result[2] * $width + $result[1] -ne $target) { throw 'Scan missed a target at a boundary' }
    }
}
$map = New-Object byte[] 48
$result = Find-TestBlock 8 6 2 $map
if ($result[0]) { throw 'Empty map incorrectly matched' }
for ($y = 4; $y -lt 6; $y++) { for ($x = 6; $x -lt 8; $x++) { $map[$y * 8 + $x] = 1 } }
$result = Find-TestBlock 8 6 2 $map
if (!$result[0] -or $result[1] -ne 6 -or $result[2] -ne 4) { throw 'Multi-pixel block at edge was missed' }
Write-Output 'PASS: center-first scan, outward priority, rectangular and narrow client coverage, empty map, edge block.'