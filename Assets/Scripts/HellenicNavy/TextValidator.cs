using TMPro;

public class TextValidator : TMP_InputValidator
{
    public override char Validate(ref string text, ref int pos, char ch)
    {
        if((ch >='0' && ch <= '9') | ch == ',') 
        {
            text += ch;
            pos++;
            return ch;
        }
        return (char)0;
    }
}
