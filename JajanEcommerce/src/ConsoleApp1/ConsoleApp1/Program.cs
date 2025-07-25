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

int[] arr = new int[] { 3, 2, 1, 6 };
int s = 0;
string a = "";
for (int i = 1; i < arr.Length; i++)
{
    if (s == 2)
    {
        break;
    }
    a += arr[i].ToString();
    s++;
}
var x = arr.Where(x => x == arr[0]).Take(2);
var res = Convert.ToInt32(a);
Console.WriteLine(""); // Output: ,3,2,0,6