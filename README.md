# .Net TextTable
Make BetterConsoleTables behave like Pythons texttable Lib

This is a simple class that wraps BetterTextTables to make it behave like Pythons texttable library. 

Essentially, BetterTextTables does not support multiple line text in rows and infact displays such output very poorly. This code analyzes each value in an added row and if a row contains a string with multiple lines it will span the row across multiple rows.

If you try this code, please let me know if you have any suggestions and I'll be glad to update. I will update this code as I find any issues in production.
