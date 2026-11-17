
$ErrorActionPreference = "Stop"
$word = New-Object -ComObject Word.Application
$word.Visible = $false
$word.DisplayAlerts = 0
try {
    $doc = $word.Documents.Open("E:\Quanap\docs\_report_build.html", [ref]$false, [ref]$false)
    $doc.SaveAs2("E:\Quanap\docs\BaoCao_BaiTapLon.docx", 16)
    $doc.Close(0)
    Write-Output "SAVED E:\Quanap\docs\BaoCao_BaiTapLon.docx"
} finally {
    $word.Quit()
    [System.Runtime.InteropServices.Marshal]::ReleaseComObject($word) | Out-Null
}
