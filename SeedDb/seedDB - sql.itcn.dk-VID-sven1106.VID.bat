for %%G in (*.sql) do sqlcmd -S sql.itcn.dk\VID -U sven1106.VID -P Tj8pV22hC8 -d sven1106.VID -i"%%G"
pause