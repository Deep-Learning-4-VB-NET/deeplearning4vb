Imports System

''' <summary>Equivalents of the java.lang.Math helpers used by the ported code.</summary>
Friend Module MathHelper
	Private ReadOnly randomInstance As New Random()

	''' <summary>Thread-safe equivalent of Java's Math.random().</summary>
	Public ReadOnly Property NextDouble() As Double
		Get
			SyncLock randomInstance
				Return randomInstance.NextDouble()
			End SyncLock
		End Get
	End Property

	''' <summary>exp(x) - 1, accurate for small |x| where Math.Exp(x) - 1 loses precision.</summary>
	Public Function Expm1(ByVal x As Double) As Double
		If Math.Abs(x) < 0.00001 Then
			Return x + 0.5 * x * x + x * x * x / 6.0
		Else
			Return Math.Exp(x) - 1.0
		End If
	End Function

	''' <summary>log(1 + x), accurate for small |x| (Kahan's correction of Math.Log(1 + x)).</summary>
	Public Function Log1p(ByVal x As Double) As Double
		Dim u As Double = 1.0 + x
		If u = 1.0 Then
			Return x
		End If
		Return Math.Log(u) * x / (u - 1.0)
	End Function
End Module
