using System;

namespace Healthcare.Engineering.Services;

public class ChaosGenerator
{
    public void RollTheDice()
    {
        var failureDice = new Random().Next(2);
        if (failureDice < 1) 
            throw new Exception("Chaos created - sorry");
    }
}