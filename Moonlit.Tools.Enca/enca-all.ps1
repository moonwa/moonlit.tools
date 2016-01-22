$fileList = Get-ChildItem 'C:\\workspace\\moonlit.tools\\moonlit.tools' -recurse * | %{$_.FullName}
Foreach($file in $fileList)
{
    .\enca "$file"
}