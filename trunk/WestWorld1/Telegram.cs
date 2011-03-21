using System;
using System.Collections.Generic;

public class Telegram
{
    public double Time { get; set; }
    public int SenderID { get; set; }
    public int ReceiverID { get; set; }
    public int MessageID { get; set; }
    
    public Telegram()
    {
        Time = -1;
        SenderID = -1;
        ReceiverID = -1;
        MessageID = -1;
    }

    public Telegram(double time, int senderID, int receiverID, int messageID)
    {
        this.Time = time;
        this.SenderID = senderID;
        this.ReceiverID = receiverID;
        this.MessageID = messageID;
    }
}
