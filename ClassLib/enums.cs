

namespace ClassLib.enums
{
    
    public enum SideOfLine
    {
        LEFT = 0,
        RIGHT = 1,
        ONTHELINE = 2
    }

    public enum CurveType
    {
        HAP = 0,//horizontal angle point
        IC = 1,//idenpendent curve
        CC = 2,//compoenent of compound curve
        RC = 3,//reverse curve
        TS = 4,//transition
        TG = 5,//tengent
        RTG = 6,//tengent with different direction
    }

    public enum BasicCurveType
    {
        CONSECUCOMPCURV = 9,//a same side CURVE with no beginning tangent following a CURVE
        REVSCURV = 10,//reverse curve following a CURVE
        CURV = 11,//single curve, component of compound curve, and reverse curve
        HAP = 12,//horizontal angle point
        TG = 14,//tengent
        RTG = 15,//tengent with different direction
    }


    public enum CurveDirection
    {
        LEFT_CURVE = 0,
        RIGHT_CURVE = 1,
    }

    public enum AboveBelowLine
    {
        ABOVE = 10,
        BELOW = 11,
        ONTHELINE = 12
    }

}