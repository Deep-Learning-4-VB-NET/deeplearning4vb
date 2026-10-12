Imports System

Friend Module MathHelper
	Private randomInstance As Random = Nothing

	Public ReadOnly Property NextDouble() As Double
		Get
			If randomInstance Is Nothing Then
				randomInstance = New Random()
			End If

			Return randomInstance.NextDouble()
		End Get
	End Property

	Public Function Expm1(ByVal x As Double) As Double
		If Math.Abs(x) < 0.00001 Then
			Return x + 0.5 * x * x
		Else
			Return Math.Exp(x) - 1.0
		End If
	End Function

	Public Function Log1p(ByVal x As Double) As Double
		Dim y As Double = x
		Return If((1 + y) = 1, y, y * (Math.Log(1 + y) / ((1 + y) - 1)))
	End Function
End Module