using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnitySQLite.Utilities;

public class TextValidator : TMP_InputValidator
{
    public override char Validate(ref string text, ref int pos, char ch)
    {
        if ((ch >= '0' && ch <= '9'))
        {
            text += ch;
            pos++;
            return ch;
        }
        return (char)0;
    }
}
public class TextValidatorNameInput : TMP_InputValidator
{
    public override char Validate(ref string text, ref int pos, char ch)
    {
        if (ch == '.' | ch == ',')
        {
            return (char)0;
        }
        else
        {
            text += ch;
            pos++;
            return ch;
        }
    }
}

public class TextValidatorDateTime : TMP_InputValidator
{
    private static List<int> longMonths = new List<int>() { 1, 3, 5, 7, 8, 10, 12 };
    private static List<string> longMonthNames = new List<string>() { "Ιανουάριος", "Μάρτιος", "Μάιος", "Ιούλιος", "Αύγουστος", "Οκτώβριος", "Δεκέμβριος" };

    private static List<int> shortMonths = new List<int>() { 4, 6, 9, 11 };
    private static List<string> shortMonthNames = new List<string>() { "Απρίλιος", "Ιούνιος", "Σεπτέμβριος", "Νοέμβριος" };

    public override char Validate(ref string text, ref int pos, char ch)
    {
        //ensure only 10 chars (year is 4, month is 2, date is 2 and 2 dashes)
        if (pos == 8) { return (char)0; }
        if ((ch >= '0' && ch <= '9') | ch == '-')
        {
            text += ch;
            pos++;
            return ch;
        }


        return (char)0;
    }

    public static bool CheckDateInput(string dateInput)
    {
        //parse string and check 
        //split at '-' should have length 3
        //[0] should have length 2 and int.Parse should be between 1 and 31, depending on month
        //[1] should have length 2 and int.Parse should be between 1 and 12
        //[2] should have length 2 
        string[] split = dateInput.Split('-');
        if (split.Length != 3)
        {
            //wrong date format, should be 3
            MessageBox.Instance.ShowMessageBox(new MessageBoxSettings()
            {
                showLabel = true,
                label = "Ημερομηνία",
                mainText = "Λάθος μορφοποίηση ημερομηνίας. Πρέπει να είναι μμ-ΜΜ-ΧΧ (ημέρα 2 ψηφία, μήνας 2 ψηφία, χρόνος 2 ψηφία).",
                useLeftButton = false,
                useRightButton = true,
                rightButtonLabel = "OK",
                onRightButtonClick = () => MessageBox.Instance.HideMessageBox(),
                showLoadingIndicator = false
            }, 15f);
            return false;
        }

        int day = int.Parse(split[0]);
        int month = int.Parse(split[1]);
        int year = int.Parse(split[2]);

        if (split[0].Length != 2)
        {
            //wrong year format, should be 4
            MessageBox.Instance.ShowMessageBox(new MessageBoxSettings()
            {
                showLabel = true,
                label = "Ημερομηνία",
                mainText = string.Format("Λάθος μορφοποίηση ημέρας ({0}). Πρέπει να έχει 2 ψηφία.", split[0]),
                useLeftButton = false,
                useRightButton = true,
                rightButtonLabel = "OK",
                onRightButtonClick = () => MessageBox.Instance.HideMessageBox(),
                showLoadingIndicator = false
            }, 15f);
            return false;
        }
        else if (split[1].Length != 2)
        {
            //wrong month format, should be 2
            MessageBox.Instance.ShowMessageBox(new MessageBoxSettings()
            {
                showLabel = true,
                label = "Ημερομηνία",
                mainText = string.Format("Λάθος μορφοποίηση μήνα ({0}). Πρέπει να έχει 2 ψηφία.", split[1]),
                useLeftButton = false,
                useRightButton = true,
                rightButtonLabel = "OK",
                onRightButtonClick = () => MessageBox.Instance.HideMessageBox(),
                showLoadingIndicator = false
            }, 15f);
            return false;
        }
        else if (split[2].Length != 2)
        {
            //wrong day format, should be 2
            MessageBox.Instance.ShowMessageBox(new MessageBoxSettings()
            {
                showLabel = true,
                label = "Ημερομηνία",
                mainText = string.Format("Λάθος μορφοποίηση χρόνου ({0}). Πρέπει να έχει 2 ψηφία.", split[2]),
                useLeftButton = false,
                useRightButton = true,
                rightButtonLabel = "OK",
                onRightButtonClick = () => MessageBox.Instance.HideMessageBox(),
                showLoadingIndicator = false
            }, 15f);
            return false;
        }

        if (month > 12 | month < 1)
        {
            //wrong month, should be between 1 and 12
            MessageBox.Instance.ShowMessageBox(new MessageBoxSettings()
            {
                showLabel = true,
                label = "Ημερομηνία",
                mainText = string.Format("Λάθος μορφοποίηση μήνα ({0}). Πρέπει να είναι μεταξύ 1 και 12, συμπεριλαμβανομένων των ορίων.", split[1]),
                useLeftButton = false,
                useRightButton = true,
                rightButtonLabel = "OK",
                onRightButtonClick = () => MessageBox.Instance.HideMessageBox(),
                showLoadingIndicator = false
            }, 15f);
            return false;

        }
        else
        {
            if (day < 1 | day > 31)
            {
                //day cannot be 0
                MessageBox.Instance.ShowMessageBox(new MessageBoxSettings()
                {
                    showLabel = true,
                    label = "Ημερομηνία",
                    mainText = string.Format("Λάθος μορφοποίηση ημέρας ({0}). Πρέπει να είναι μεταξύ 1 και 31, αναλόγως με τον μήνα.", split[0]),
                    useLeftButton = false,
                    useRightButton = true,
                    rightButtonLabel = "OK",
                    onRightButtonClick = () => MessageBox.Instance.HideMessageBox(),
                    showLoadingIndicator = false
                }, 15f);
                return false;
            }
            //check day depending on month
            //february
            if (month == 2)
            {
                if (day > 28)
                {
                    //wrong day format
                    MessageBox.Instance.ShowMessageBox(new MessageBoxSettings()
                    {
                        showLabel = true,
                        label = "Ημερομηνία",
                        mainText = string.Format("Λάθος μορφοποίηση ημέρας ({0}). Ο μήνας Φεβρουάριος έχει 28 ημέρες.", split[0]),
                        useLeftButton = false,
                        useRightButton = true,
                        rightButtonLabel = "OK",
                        onRightButtonClick = () => MessageBox.Instance.HideMessageBox(),
                        showLoadingIndicator = false
                    }, 15f);
                    return false;
                }
            }
            else if (shortMonths.Contains(month))
            {

                if (day > 30)
                {
                    //wrong day format
                    MessageBox.Instance.ShowMessageBox(new MessageBoxSettings()
                    {
                        showLabel = true,
                        label = "Ημερομηνία",
                        mainText = string.Format("Λάθος μορφοποίηση ημέρας ({0}). Ο μήνας {1} έχει 30 ημέρες.", split[0], shortMonthNames[shortMonths.IndexOf(month)]),
                        useLeftButton = false,
                        useRightButton = true,
                        rightButtonLabel = "OK",
                        onRightButtonClick = () => MessageBox.Instance.HideMessageBox(),
                        showLoadingIndicator = false
                    }, 15f);
                    return false;
                }
            }
            else if (longMonths.Contains(month))
            {
                if (day > 31)
                {
                    //wrong day format
                    MessageBox.Instance.ShowMessageBox(new MessageBoxSettings()
                    {
                        showLabel = true,
                        label = "Ημερομηνία",
                        mainText = string.Format("Λάθος μορφοποίηση ημέρας ({0}). Ο μήνας {1} έχει 31 ημέρες.", split[0], longMonthNames[longMonths.IndexOf(month)]),
                        useLeftButton = false,
                        useRightButton = true,
                        rightButtonLabel = "OK",
                        onRightButtonClick = () => MessageBox.Instance.HideMessageBox(),
                        showLoadingIndicator = false
                    }, 15f);
                    return false;
                }
            }
        }

        //all is well
        return true;
    }
}
