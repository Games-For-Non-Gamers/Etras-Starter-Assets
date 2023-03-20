using UnityEngine;

public static partial class RectExtensions
{
    //Resizing to side
    //Changes rect's size while snapping to the specified side
    public static Rect ResizeToTop(this Rect r, float height)
    {
        r = new Rect(r);
        r.height = height;
        return r;
    }

    public static Rect ResizeToBottom(this Rect r, float height)
    {
        r = new Rect(r);
        r.y += r.height - height;
        r.height = height;
        return r;
    }

    public static Rect ResizeToLeft(this Rect r, float width)
    {
        r = new Rect(r);
        r.width = width;
        return r;
    }

    public static Rect ResizeToRight(this Rect r, float width)
    {
        r = new Rect(r);
        r.x += r.width - width;
        r.width = width;
        return r;
    }

    //Moving
    //The rect gets moved from the specified side and resized to not overflow
    public static Rect MoveTop(this Rect r, float amount)
    {
        r = new Rect(r);
        r.y += amount;
        r.height -= amount;
        return r;
    }

    public static Rect MoveBottom(this Rect r, float amount)
    {
        r = new Rect(r);
        r.y -= amount;
        r.height -= amount;
        return r;
    }

    public static Rect MoveLeft(this Rect r, float amount)
    {
        r = new Rect(r);
        r.x -= amount;
        r.width -= amount;
        return r;
    }

    public static Rect MoveRight(this Rect r, float amount)
    {
        r = new Rect(r);
        r.x += amount;
        r.width -= amount;
        return r;
    }

    //Resizing
    public static Rect Border(this Rect rect, float border) =>
        Border(rect, border, border, border, border);

    public static Rect Border(this Rect rect, float x, float y) =>
        Border(rect, x, x, y, y);

    public static Rect Border(this Rect rect, Vector2 size) =>
        Border(rect, size.x, size.x, size.y, size.y);

    public static Rect Border(this Rect rect, float left, float right, float top, float bottom)
    {
        rect.x += left;
        rect.width -= left + right;
        rect.y += top;
        rect.height -= top + bottom;
        return rect;
    }

    public static Rect BorderTop(this Rect rect, float amount) =>
        Border(rect, 0f, 0f, amount, 0f);

    public static Rect BorderBottom(this Rect rect, float amount) =>
        Border(rect, 0f, 0f, 0f, amount);

    public static Rect BorderLeft(this Rect rect, float amount) =>
        Border(rect, amount, 0f, 0f, 0f);

    public static Rect BorderRight(this Rect rect, float amount) =>
        Border(rect, 0f, amount, 0f, 0f);

    /// <summary>Create horizontal border while preserving rect's aspect ratio</summary>
    public static Rect BorderAspectRatioHorizontal(this Rect rect, float amount) =>
        Border(rect, amount, rect.height / rect.width * amount);

    /// <summary>Create vertical border while preserving rect's aspect ratio</summary>
    public static Rect BorderAspectRatioVertical(this Rect rect, float amount) =>
        Border(rect, rect.width / rect.height * amount, amount);

    //Simple actions
    public static Rect MoveX(this Rect rect, float amount)
    {
        rect.x += amount;
        return rect;
    }

    public static Rect MoveY(this Rect rect, float amount)
    {
        rect.y += amount;
        return rect;
    }

    public static Rect SetWidth(this Rect rect, float amount)
    {
        rect.width = amount;
        return rect;
    }

    public static Rect SetHeight(this Rect rect, float amount)
    {
        rect.height = amount;
        return rect;
    }

    public static Rect ScaleX(this Rect rect, float size)
    {
        rect.width *= size;
        return rect;
    }

    public static Rect ScaleY(this Rect rect, float size)
    {
        rect.height *= size;
        return rect;
    }
}
