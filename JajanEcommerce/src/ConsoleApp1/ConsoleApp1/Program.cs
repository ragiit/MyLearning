string input1 = "5000000";
string input2 = "15";
string input3 = "10";
//WRITE YOUR CODE HERE

if (!int.TryParse(input1, out int _) || !int.TryParse(input2, out int __) || !int.TryParse(input3, out int ___))
{
    Console.WriteLine("Wrong Input");
}
else
{
    double total = Convert.ToInt32(input1);
    for (int i = 0; i < Convert.ToInt32(input3); i++)
    {
        total += total * (Convert.ToDouble(input2) / 100);
    }

    Console.WriteLine($"Rp {Math.Floor(total)}");
}

//var strArr = new string[] { "1, 3, 4, 7, 13", "1, 2, 4, 13, 15" };
//var a = strArr.Distinct().ToList();
//List<string> list = new List<string>();
//foreach (var i in a)
//{
//    var splits = i.Split(", ");
//    var bsplit = a[1].Split(", ");
//    foreach (var x in splits)
//    {
//        if (string.IsNullOrWhiteSpace(x))
//        {
//            //return "EMPTY";
//        }
//        var cek = bsplit.Contains(x);
//        if (cek)
//        {
//            list.Add(x);
//        }
//    }
//    break;
//}
//var result = "";
//foreach (var i in list)
//{
//    result += $"{i},";
//}
//result = string.Join(",", list);
//var parts = result.Split(',');

//var resultz = parts
//         .Skip(1)
//         .Select(x =>
//         {
//             var removed = x.Replace("1", "");
//             return string.IsNullOrEmpty(removed) ? "0" : removed;
//         });

//string output = "," + string.Join(",", resultz);
//var aa = "";

//int[] arr = new int[] { 3, 2, 1, 6 };
//int s = 0;
//string a = "";
//for (int i = 1; i < arr.Length; i++)
//{
//    if (s == 2)
//    {
//        break;
//    }
//    a += arr[i].ToString();
//    s++;
//}
//var x = arr.Where(x => x == arr[0]).Take(2);
//var res = Convert.ToInt32(a);
//Console.WriteLine(""); // Output: ,3,2,0,6

// Array Challange
//var strArr = new string[] { "1, 3, 4, 7, 13", "1, 2, 4, 13, 15" };
//var strArr = new string[] { "1, 3, 9, 10, 17, 18", "1, 4, 9, 10" };
//List<string> list = new List<string>();
//var data1 = strArr[0].Split(", ");
//var data2 = strArr[1].Split(", ");
//foreach (var item in data1)
//{
//    var c = data2.Where(x => x == item).FirstOrDefault();
//    if (c != null)
//    {
//        list.Add(item);
//    }
//}

//var result = string.Join(", ", list);
//var r = "";
//foreach (var item in list)
//{
//    if (item.ToString().Length > 1)
//    {
//        for (int z = 0; z < item.ToString().Length; z++)
//        {
//            r += $"{item.ToString()[z]}--";
//        }
//    }
//    else
//    {
//        r += $"{item.ToString()},";
//    }
//}

//var aa = "";

//var st = new int[] { 3, 2, 1, 6 };
//var st = new int[] { 4, 3, 4, 3, 1, 2 };
//var a = st.Skip(st[0]).ToList();
//var b = st.Skip(1).Take(st[0] - 1);
//var bb = new List<int>();
//for (int i = 0; i < st[0]; i++)
//{
//    a.Add(st[i]);
//}
//var aaa = a;
//var ss = string.Join("", aaa);
//var x = Convert.ToInt32(string.Join("", a));
//Console.ReadKey();

// CAT
//kaamvjjfl, 4

//k          j
// a       j  f
//   a   v     l
//     m

//kjajfavlm
//var arr = new string[] { "cat", "5" };
//var arr = new string[] { "kaamvjjfl", "4" };
//var data1 = arr[0];
//var data2 = Convert.ToInt32(arr[1]);
//var r = "";
//if (data1.Length > data2)
//{
//    r = data1.ToString();
//}

//var temp = new List<Temp>();
//var leng = 1;
//var isPlus = false;
//for (int i = 0; i < data1.Length; i++)
//{
//    temp.Add(new Temp
//    {
//        Text = data1[i].ToString(),
//        Line = leng,
//    });

//    if (leng == data2)
//        isPlus = false;
//    else
//    {
//        if (leng != 1 && !isPlus)
//        {
//            isPlus = false;
//        }
//        else
//        {
//            isPlus = true;
//        }
//    }

//    if (isPlus)
//    {
//        leng += 1;
//    }
//    else
//    {
//        leng -= 1;
//    }
//}

//var result = "";

//temp.OrderBy(x => x.Line).ToList().ForEach(x =>
//{
//    result += $"{x.Text}";
//});

//var a = result;
//Console.ReadLine();

//internal class Temp
//{
//    public string Text { get; set; }
//    public int Line { get; set; }
//}

// Searching Challange
//var s = "abcde";
////var s = "hello world hi hey";
////var temp = new List<Temp>();
////for (int i = 0; i < s.Length; i++)
////{
////    var c = s[i].ToString();
////    if (string.IsNullOrWhiteSpace(c))
////        continue;

////    var cek = temp.FirstOrDefault(x => x.Text == c);
////    if (cek != null)
////    {
////        cek.Count += 1;
////    }
////    else
////    {
////        temp.Add(new Temp { Text = c, Count = 1 });
////    }
////}

////var result = temp.OrderBy(x => x.Count).FirstOrDefault()?.Text ?? "EMPTY";
////var e = "";

//internal class Temp
//{
//    public string Text { get; set; }
//    public int Count { get; set; }
//}