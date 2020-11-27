using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TestNetMessage : GameMonoBehaviour
{
    public override void OnGameAwake()
    {
        base.OnGameAwake();

        SessionInterface sessionInterface = OnlineService.OnlineInterface?.SessionInterface;
        if (sessionInterface != null)
        {
            sessionInterface.RegisterNetMessageReceiver<TestMessage>(OnReceiveMessage);
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        SessionInterface sessionInterface = OnlineService.OnlineInterface?.SessionInterface;
        if (sessionInterface != null)
        {
            sessionInterface.UnregisterNetMessageReceiver<TestMessage>(OnReceiveMessage);
        }
    }

    [ConsoleCommand(Description = "Send a complexe Net message to your peers to test the system.")]
    private static void SendTestNetMessage()
    {
        SessionInterface sessionInterface = OnlineService.OnlineInterface?.SessionInterface;

        if (sessionInterface != null)
        {

            TestMessage msg = new TestMessage();

            msg.valueString = "test"; // 8 + 2
            msg.valueInt = -10;       // 4
            msg.valueUInt = 50;       // 4
            msg.valueShort = -56;     // 2
            msg.valueUShort = 33;     // 2
            msg.valueBool = true;     // 0.2
            msg.valueByte = 12;       // 1
            msg.arrayOfInts = new int[] { 0, 1, 2 };
            msg.listOfInts = new List<int> { 0, 1, 2 };


            msg.cat = new TestMessageCat("FirstCat", 9);
            msg.dog = new TestMessageDog("FirstDog", true);
            msg.animal = new TestMessageDog("SurpriseDog!", false);
            msg.animals = new TestMessageAnimal[]
            {
                new TestMessageAnimal("Giraffe")
                , new TestMessageCat("Moustache", 99)
                , new TestMessageDog("Bento", true)
            };

            sessionInterface.SendNetMessage(msg, sessionInterface.Connections);
        }
    }

    void OnReceiveMessage(TestMessage msg, INetworkInterfaceConnection sender)
    {
        DebugScreenMessage.DisplayMessage("Received TestMessage:\n" + msg);
    }
}

[NetSerializable]
public class TestMessage
{
    public string valueString;
    public int valueInt;
    public uint valueUInt;
    public short valueShort;
    public ushort valueUShort;
    public bool valueBool;
    public byte valueByte;
    public int[] arrayOfInts;
    public List<int> listOfInts;
    public TestMessageCat cat;
    public TestMessageDog dog;
    public TestMessageAnimal animal;
    public TestMessageAnimal[] animals;


    [NotNetSerialized]
    public bool ignoreThisField;


    public override string ToString()
    {
        StringBuilder str = new StringBuilder();
        str.AppendLine(valueString);
        str.AppendLine(valueInt.ToString());
        str.AppendLine(valueUInt.ToString());
        str.AppendLine(valueShort.ToString());
        str.AppendLine(valueUShort.ToString());
        str.AppendLine(valueBool.ToString());
        str.AppendLine(valueByte.ToString());
        str.AppendLine(valueInt.ToString());

        foreach (var integer in arrayOfInts)
        {
            str.AppendLine(integer.ToString());
        }

        foreach (var integer in listOfInts)
        {
            str.AppendLine(integer.ToString());
        }

        str.AppendLine(cat.ToString());
        str.AppendLine(dog.ToString());
        str.AppendLine(animal.ToString());

        foreach (var animal in animals)
        {
            str.AppendLine(animal.ToString());
        }

        return str.ToString();
    }
}

[NetSerializable(IsBaseClass = true)]
public class TestMessageAnimal
{
    public TestMessageAnimal() { }
    public TestMessageAnimal(string name) { this.name = name; }
    public string name;

    public override string ToString()
    {
        return "Animal[" + name + "]";
    }
}

[NetSerializable]
public class TestMessageCat : TestMessageAnimal
{
    public int numberOfLivesLeft;

    public TestMessageCat() { }
    public TestMessageCat(string name, int numberOfLivesLeft)
        : base(name)
    {
        this.numberOfLivesLeft = numberOfLivesLeft;
    }

    public override string ToString()
    {
        return "Cat[" + name + "] with " + numberOfLivesLeft + " lives left";
    }
}

[NetSerializable]
public class TestMessageDog : TestMessageAnimal
{
    public bool isAGoodBoy;

    public TestMessageDog() { }
    public TestMessageDog(string name, bool isAGoodBoy)
        : base(name)
    {
        this.isAGoodBoy = isAGoodBoy;
    }

    public override string ToString()
    {
        return "Dog[" + name + "] " + (isAGoodBoy ? "is a good boy" : "is not a good boy.");
    }
}