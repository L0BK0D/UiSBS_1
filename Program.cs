using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections.Specialized;
using System.Collections;

public static class Program
{
    // Ниже - bitArray

    static int startGrade;
    public static BitArray dividedByBit(BitArray poly1, BitArray poly2) // poly1 - делимое, poly2 - делитель
    {
        int elderIndexPoly1 = -1, elderIndexPoly2 = -1;
        BitArray endBeatsTemp = new BitArray(poly1.Count);
        BitArray tempPoly = new BitArray(poly2.Count);
        for (int i = 0; i < tempPoly.Count; i++)
            tempPoly[i] = poly2[i];
        for (int i = 0; i < endBeatsTemp.Count; i++)
            endBeatsTemp[i] = poly1[i];
        BitArray mod = new BitArray(Math.Max(endBeatsTemp.Count, poly2.Count));// Остаток от деления в этой ссылке
        for (int i = 0; i < endBeatsTemp.Count; i++)
        {
            if (endBeatsTemp[i].Equals(true) && elderIndexPoly1 == -1)
                elderIndexPoly1 = i;
            if (poly2[i].Equals(true) && elderIndexPoly2 == -1)
                elderIndexPoly2 = i;
            if (elderIndexPoly2 != -1 && elderIndexPoly1 != -1)
                break;
        }
        if(elderIndexPoly1 > elderIndexPoly2 || elderIndexPoly1 == -1)
        {
            mod = endBeatsTemp;
            return mod;
        }
        else
        {
            tempPoly.RightShift(elderIndexPoly2 - elderIndexPoly1);
            endBeatsTemp.Xor(tempPoly);
            return dividedByBit(endBeatsTemp, poly2);
        }
            
    }
    //
    public static BitArray polyShiftForGrade(BitArray polyforGrade, int startGrade)
    {
        int myLength = polyforGrade.Length + startGrade;
        BitArray endBits = new BitArray(myLength);
        for (int i = 0; i < myLength; i++)
        {
            if (i < myLength - polyforGrade.Length)
                endBits[i] = false;
            else
                endBits[i] = Convert.ToBoolean(polyforGrade[i - myLength + polyforGrade.Length]);
        }
        endBits.RightShift(startGrade);
        return endBits;
    }
    //Перевод из массива чисел в массив логических значений (тип BitArray)
    public static BitArray primPolyToLength(int[] primPoly, int length)
    {
        // PrimPoly = [1, 0, 1, 1]
        startGrade = primPoly.Length - 1;
        BitArray newBit = new BitArray(length); // например length = 7 
        for(int i = 0; i < length; i++)
        {
            if(i < length - primPoly.Length)
                newBit[i] = false;
            else
                newBit[i] = Convert.ToBoolean(primPoly[i - length + primPoly.Length]);
        }
        return newBit;

    }
    //Основная функция кодера
    public static List<BitArray> codedSequences(BitArray primPoly, int length)
    {

        List<BitArray> binSeqs = binSequences(length);
        List<BitArray> codedSeqs = new List<BitArray>();
        foreach(var seq in binSeqs)
        {
            BitArray gradedBits = polyShiftForGrade(seq, startGrade);
            codedSeqs.Add(gradedBits.Xor(dividedByBit(gradedBits, primPoly)));
            //BitArray test = dividedByBit(gradedBits, primPoly);
        }
        return codedSeqs;
    }
    //Генерация двоичных чисел длины k
    public static List<BitArray> binSequences(int length)
    {
        List<BitArray> seqList = new List<BitArray>();        
        for(int i = 0; i < (int)Math.Pow(2, length); i++)
        {
            seqList.Add(new BitArray(Array.ConvertAll(Convert.ToString(i, 2).PadLeft(length, '0').ToArray(), c => Convert.ToBoolean(Char.GetNumericValue(c))))); 
            // Все массивы с многочленами передаются в List, идет на выход.
        }
        return seqList;
    }
    //Функция возведения в степень
    public static double RaiseToPow(double x, int power)
    {
        double res;
        int i;
        res = 1.0;
        if (power == 0)
        {return 1;}
        else if (power == 1)
        {return x;}
        else
            for (i = 1; i <= power; i++)
            {res = res * x;}
        return res;
    }
    //Вычисление факториала числа
    public static int Fact(int k)
    {
        if (k < 2)
            return 1;
        return k * Fact(k - 1);
    }
    //Функци подсчета кодовых слов с заданным количеством единиц
    static int oneCounter(int k, int i, int n, int[] g) // i - количество единиц в конечной последовательности, n - длина выходной последовательности, r - степень примитивного многочлена, d - минимальное расстояние
    {
        int resultCounter = 0;
        List<BitArray> list = codedSequences(primPolyToLength(g, n), k);
        /*foreach (var seq in list)
        {

            foreach (var bit in seq)
            {
                int a = Convert.ToInt16(bit);
                Console.Write(a);
            }
            Console.WriteLine();
        }*/
        foreach (BitArray b in list)
        {
            if (b.OfType<bool>().Count(b => b) == i)
                resultCounter++;
        }
        return resultCounter;
    }

    //Функция основных вычислений
    public static void Prob_ab()
    {
        int  k, d;
        int[] g; // String 1011 -> BitArray [ 1, 0, 1, 1 ]
        double p;
        //p=0.01;
        //Введите порождающий вектор
        Console.Write("g(x)|g = ");
        g = Array.ConvertAll(Console.ReadLine().ToCharArray(), c => (int)Char.GetNumericValue(c));
        //Введите длину кодируемой последовательности
        Console.Write(" \n k = ");
        k = Convert.ToInt32(Console.ReadLine());
        //Введите минимальное расстояние кода
        Console.Write(" \n d = ");
        d = Convert.ToInt32(Console.ReadLine());
        //Введите вероятность ошибки в канале
        Console.Write("\n p = ");
        p=Convert.ToDouble(Console.ReadLine());
        int i;
        double sum = 0;    

        int r = g.Length - 1;
        // int r = sizeof(g) - 1;
        int n = k + r;
        //for (int j = 0; j < 100; j++)
        {
            sum = 0;
            //Вычисление верхней оценки ошибки декодирования
            for (i = 0; i <= d - 1; i++)
            {
                sum += (Fact(n) / Fact(i) / Fact(n - i)) * RaiseToPow(p, i) * RaiseToPow(1 - p, n - i);
            }
            Console.Write("\n P+e = ");Console.WriteLine(1 - sum);

            //Вычисление точного значения ошибки декодирования
            sum = 0;
            for (i = d; i <= n; i++)
            {
                //sum += RaiseToPow(2, k) * RaiseToPow(p, i) * RaiseToPow(1 - p, n - i);
                sum += oneCounter(k, i, n, g) * RaiseToPow(p, i) * RaiseToPow(1 - p, n - i);
            }
            Console.Write("\n Pe = ");Console.WriteLine(sum);
            //p+=0.01;
            //Math.Round(p, 2);
        }
    }
    //Основная функция вызывает функцию вычислений
    static void Main(string[] args)
    {
        Prob_ab();
    }
}
