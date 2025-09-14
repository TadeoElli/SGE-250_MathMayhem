using UnityEngine;
public struct Fraction
{
    public int numerator;
    public int denominator;

    public Fraction(int n, int d)
    {
        if (d == 0)
        {
            throw new System.DivideByZeroException("Denominator cannot be zero.");
        }

        numerator = n;
        denominator = d;
        Simplify();
    }

    public float ToFloat() => (float)numerator / denominator;
    // Simplifica la fracción reduciéndola al mínimo común divisor
    public void Simplify()
    {
        int gcd = GCD(Mathf.Abs(numerator), Mathf.Abs(denominator));
        numerator /= gcd;
        denominator /= gcd;

        // Asegura que el denominador siempre sea positivo
        if (denominator < 0)
        {
            denominator = -denominator;
            numerator = -numerator;
        }
    }

    // Máximo común divisor (Euclides)
    private static int GCD(int a, int b)
    {
        while (b != 0)
        {
            int temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }

    public static Fraction operator +(Fraction a, Fraction b)
    {
        int n = a.numerator * b.denominator + b.numerator * a.denominator;
        int d = a.denominator * b.denominator;
        return new Fraction(n, d);
    }

    public static Fraction operator -(Fraction a, Fraction b)
    {
        int n = a.numerator * b.denominator - b.numerator * a.denominator;
        int d = a.denominator * b.denominator;
        return new Fraction(n, d);
    }

    public static Fraction operator *(Fraction a, Fraction b)
    {
        int n = a.numerator * b.numerator;
        int d = a.denominator * b.denominator;
        return new Fraction(n, d);
    }

    public static Fraction operator /(Fraction a, Fraction b)
    {
        if (b.numerator == 0)
            throw new System.DivideByZeroException("Cannot divide by zero fraction.");

        int n = a.numerator * b.denominator;
        int d = a.denominator * b.numerator;
        return new Fraction(n, d);
    }

    public override string ToString()
    {
        if (denominator == 1)
            return numerator.ToString();
        return $"{numerator}/{denominator}";
    }
}
