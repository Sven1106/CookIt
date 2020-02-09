for /r %%G in (*.sql) do SQLCMD -S (localdb)\MSSQLLocalDB -d CookIt -i"%%G"
pause