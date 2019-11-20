for %%G in (*.sql) do sqlcmd -S (localdb)\MSSQLLocalDB -U "" -P "" -d DanNet -i"%%G"
pause