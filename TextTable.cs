using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

public class TextTable : BetterConsoleTables.Table
{
    private Alignment _head_align = Alignment.Left;
    private Alignment _rows_align = Alignment.Left;
    private MethodInfo _mAddRow;

    public TextTable(Alignment head_align = Alignment.Left, Alignment rows_align = Alignment.Left, bool tc_simple = true) : base()
    {
        this._head_align = head_align;
        this._rows_align = rows_align;
        var tc = new TableConfiguration();
        this.Config = IIf(tc_simple, TableConfiguration.Simple, TableConfiguration.Markdown);
        this._mAddRow = this.GetType().GetMethod("AddRow", BindingFlags.Instance | BindingFlags.Public, null/* TODO Change to default(_) if this is not a reference type */,
        {
            typeof(object[])
        }, null/* TODO Change to default(_) if this is not a reference type */);
    }
    public void header(params object[] args)
    {
        MethodInfo m;
        m = this.GetType()
                .GetMethod("AddColumns", BindingFlags.Instance | BindingFlags.Public, null/* TODO Change to default(_) if this is not a reference type */,
        {
            typeof(Alignment),
            typeof(Alignment),
            typeof(object[])
        }, null/* TODO Change to default(_) if this is not a reference type */);
        try
        {
            m.Invoke(this,
            {
                this._rows_align,
                this._head_align,
                args
            });
        }
        catch (TargetInvocationException tie)
        {
            if (tie.InnerException != null)
                throw tie.InnerException;
            throw;
        }
    }

    public void add_row<T>(T[] row)
    {
        int iNewLineCnt = -1;
        var aVals = new object[row.length + 1];
        object oVal;
        for (int i = 0; i <= row.length - 1; i++)
        {
            oVal = (object)row[i];
            if (oVal is string)
            {
                var sVal = CTypeDynamic<string>(oVal);
                sVal = sVal.replace(Constants.vbCr, "");
                if (sVal.Contains(Constants.vbLf))
                {
                    var a = sVal.trim().Split(Constants.vbLf);
                    if (a.length > iNewLineCnt)
                        iNewLineCnt = a.length;
                    aVals[i] = a;
                }
                else
                    aVals[i] = sVal;
            }
            else
                aVals[i] = oVal;
        }

        if (iNewLineCnt == -1)
        {
            object[] row_use = new object[row.Count - 1 + 1];
            for (int i = 0; i <= row.count - 1; i++)
                row_use[i] = row[i];
            this.add_row(row_use);
            return;
        }

        object[] aRow_c;
        for (int i = 0; i <= iNewLineCnt - 1; i++)
        {
            aRow_c = new object[row.length - 1 + 1];
            for (int j = 0; j <= row.length - 1; j++)
            {
                oVal = aVals[j];
                if (oVal is string[])
                {
                    var a = (string[])oVal;
                    if (i < a.length)
                        aRow_c[j] = a[i].TrimEnd();
                    else
                        aRow_c[j] = "";
                }
                else
                    aRow_c[j] = iif(i == 0, oVal, "");
            }
            this.add_row(aRow_c);
        }
    }


    public void add_row(object[] row)
    {
        if (row.Length != this.Columns.Count)
            throw new Exception($"The number columns in row given ({row.Length}) is greater than the number of columns in the table ({this.Columns.Count})");

        // Normalize values in row
        for (int i = 0; i <= row.Length - 1; i++)
        {
            if (row[i] == null)
                row[i] = "(null)";
            else if (isDBNull(row[i]))
                row[i] = "(dbnull)";
        }

        try
        {
            this._mAddRow.Invoke(this, new[] { row });
        }
        catch (TargetInvocationException tie)
        {
            if (tie.InnerException != null)
                throw tie.InnerException;
            throw;
        }
    }

    public string draw()
    {
        return this.ToString();
    }

    public string draw(int[] colLens)
    {
        return this.ToString(columnLengths: colLens);
    }

    public int Count
    {
        get
        {
            return this.Rows.Count;
        }
    }
}
