using UnityEngine;
public class MessageParser
{
    private const string varDevider = "|";
    private const string valueDevider = ":";
    private BreathingDeviceCommmunicationParserList parserList;

    public MessageParser(BreathingDeviceCommmunicationParserList parserList)
    {
        this.parserList = parserList;
    }

    public void ParseMessage(string message)
    {
        string[] messageParts = message.Split(varDevider);

        foreach (string messagePart in messageParts)
        {
            if (messagePart == "") continue;

            string[] variables = messagePart.Split(valueDevider);

            if (parserList.StringToSByte.TryGetValue(variables[0], out sbyte token))
            {
                if (parserList.DataActionDic.TryGetValue(token, out DataAction action))
                {
                    action.OnDataReceived(variables[1]);
                }
            }

        }

    }

    private static void SetBreathingData(sbyte messageToken, string value) // to do create a non string based parser.
    {

    }


}
