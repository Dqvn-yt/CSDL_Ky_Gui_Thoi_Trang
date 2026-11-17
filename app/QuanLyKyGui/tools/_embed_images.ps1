$ErrorActionPreference = "Stop"
$word = New-Object -ComObject Word.Application
$word.Visible = $false
$word.DisplayAlerts = 0
try {
    $doc = $word.Documents.Open("E:\Quanap\docs\BaoCao_BaiTapLon.docx")
    # Bề rộng vùng nội dung A4: 21 - 2.0 - 2.6 = 16.4 cm = 464.9 pt
    $target = 460.0
    $n = 0
    foreach ($shp in $doc.InlineShapes) {
        if ($shp.Type -eq 4) {
            $shp.LinkFormat.SavePictureWithDocument = $true
        }
        $shp.LockAspectRatio = -1
        $shp.Width = $target
        $n++
    }
    Write-Output ("Pictures resized: " + $n)
    $doc.Save()
    $doc.Close(0)
} finally {
    $word.Quit()
    [System.Runtime.InteropServices.Marshal]::ReleaseComObject($word) | Out-Null
}
Write-Output "DONE"
