Imports System

Friend Module DateTimeHelper
	Private ReadOnly Jan1st1970 As New Date(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
	Public Function CurrentUnixTimeMillis() As Long
		Return CLng((Date.UtcNow.Subtract(Jan1st1970).TotalMilliseconds))
	End Function
End Module