Imports System
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Pointer = org.bytedeco.javacpp.Pointer
Imports PointerPointer = org.bytedeco.javacpp.PointerPointer
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports GaussianDistribution = org.nd4j.linalg.api.ops.random.impl.GaussianDistribution
Imports UniformDistribution = org.nd4j.linalg.api.ops.random.impl.UniformDistribution
Imports Random = org.nd4j.linalg.api.rng.Random
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NativeOps = org.nd4j.nativeblas.NativeOps

'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  *  See the NOTICE file distributed with this work for additional
' *  *  information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
' 

Namespace org.nd4j.rng


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public abstract class NativeRandom implements org.nd4j.linalg.api.rng.Random
	Public MustInherit Class NativeRandom
		Implements Random

		Public MustOverride Sub setStates(ByVal rootState As Long, ByVal nodeState As Long)
		Public MustOverride Function nodeState() As Long Implements Random.nodeState
		Public MustOverride Function rootState() As Long Implements Random.rootState
		Public MustOverride Function nextLong() As Long Implements Random.nextLong
		Public MustOverride Function nextInt() As Integer Implements Random.nextInt
		Public MustOverride Property Seed As Long Implements Random.getSeed
		Protected Friend nativeOps As NativeOps
'JAVA TO VB CONVERTER NOTE: The field statePointer was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend statePointer_Conflict As Pointer

		Protected Friend currentPosition As New AtomicLong(0)

		' special stuff for gaussian
		Protected Friend z0, z1, u0, u1 As Double
		Protected Friend generated As Boolean = False
		Protected Friend mean As Double = 0.0
		Protected Friend stdDev As Double = 1.0

'JAVA TO VB CONVERTER NOTE: The field seed was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend seed_Conflict As Long

		Public Sub New()
			Me.New(DateTimeHelper.CurrentUnixTimeMillis())
		End Sub

		Public Sub New(ByVal seed As Long)
			Me.New(seed, 10000000)
		End Sub

		Public Sub New(ByVal seed As Long, ByVal numberOfElements As Long)
			Me.seed_Conflict = seed
			init()
		End Sub

		Public MustOverride Sub init()

		Public Overridable WriteOnly Property Seed Implements Random.setSeed As Integer
			Set(ByVal seed As Integer)
				Me.Seed = CLng(seed)
			End Set
		End Property

		Public Overridable WriteOnly Property Seed Implements Random.setSeed As Integer()
			Set(ByVal seed() As Integer)
				Dim sd As Long = 0
				For Each em As Integer In seed
					sd *= em
				Next em
				Me.Seed = sd
			End Set
		End Property

		Public Overridable Sub nextBytes(ByVal bytes() As SByte) Implements Random.nextBytes
			Throw New System.NotSupportedException()
		End Sub

		Public Overridable Function nextInt(ByVal [to] As Integer) As Integer Implements Random.nextInt
			Dim r As Integer = nextInt()
			Dim m As Integer = [to] - 1
			If ([to] And m) = 0 Then ' i.e., bound is a power of 2
				r = CInt(([to] * CLng(r)) >> 31)
			Else
				Dim u As Integer = r
				r = u Mod [to]
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: for (int u = r; u - (r = u % to) + m < 0; u = nextInt())
				Do While u - r + m < 0

						r = u Mod [to]
					u = nextInt()
				Loop
			End If
			Return r
		End Function

		Public Overridable Function nextInt(ByVal a As Integer, ByVal n As Integer) As Integer Implements Random.nextInt
			Return nextInt(n - a) + a
		End Function

		Public MustOverride ReadOnly Property ExtraPointers As PointerPointer

		Public Overridable Function nextBoolean() As Boolean Implements Random.nextBoolean
			Return nextInt() Mod 2 = 0
		End Function

		Public MustOverride Overrides Function nextFloat() As Single Implements Random.nextFloat

		Public MustOverride Overrides Function nextDouble() As Double Implements Random.nextDouble

		Public Overridable Function nextGaussian() As Double Implements Random.nextGaussian
			Dim epsilon As Double = 1e-15
			Dim two_pi As Double = 2.0 * 3.14159265358979323846

			If Not generated Then
				Do
					u0 = nextDouble()
					u1 = nextDouble()
				Loop While u0 <= epsilon

				z0 = Math.Sqrt(-2.0 * Math.Log(u0)) * Math.Cos(two_pi * u1)
				z1 = Math.Sqrt(-2.0 * Math.Log(u0)) * Math.Sin(two_pi * u1)

				generated = True

				Return z0 * stdDev + mean
			Else
				generated = False

				Return z1 * stdDev + mean
			End If
		End Function

		Public Overridable Function nextGaussian(ByVal shape() As Integer) As INDArray Implements Random.nextGaussian
			Return nextGaussian(Nd4j.order(), shape)
		End Function

		Public Overridable Function nextGaussian(ByVal shape() As Long) As INDArray Implements Random.nextGaussian
			Return nextGaussian(Nd4j.order(), shape)
		End Function

		Public Overridable Function nextGaussian(ByVal order As Char, ByVal shape() As Integer) As INDArray Implements Random.nextGaussian
			Dim array As INDArray = Nd4j.createUninitialized(shape, order)
			Dim op As New GaussianDistribution(array, 0.0, 1.0)
			Nd4j.Executioner.exec(op, Me)

			Return array
		End Function

		Public Overridable Function nextGaussian(ByVal order As Char, ByVal shape() As Long) As INDArray Implements Random.nextGaussian
			Dim array As INDArray = Nd4j.createUninitialized(shape, order)
			Dim op As New GaussianDistribution(array, 0.0, 1.0)
			Nd4j.Executioner.exec(op, Me)

			Return array
		End Function

		Public Overridable Function nextDouble(ByVal shape() As Integer) As INDArray Implements Random.nextDouble
			Return nextDouble(Nd4j.order(), shape)
		End Function



		Public Overridable Function nextDouble(ByVal shape() As Long) As INDArray Implements Random.nextDouble
			Return nextDouble(Nd4j.order(), shape)
		End Function

		Public Overridable Function nextDouble(ByVal order As Char, ByVal shape() As Integer) As INDArray Implements Random.nextDouble
			Dim array As INDArray = Nd4j.createUninitialized(shape, order)
			Dim op As New UniformDistribution(array, 0.0, 1.0)
			Nd4j.Executioner.exec(op, Me)

			Return array
		End Function

		Public Overridable Function nextDouble(ByVal order As Char, ByVal shape() As Long) As INDArray Implements Random.nextDouble
			Dim array As INDArray = Nd4j.createUninitialized(shape, order)
			Dim op As New UniformDistribution(array, 0.0, 1.0)
			Nd4j.Executioner.exec(op, Me)

			Return array
		End Function

		Public Overridable Function nextFloat(ByVal shape() As Integer) As INDArray Implements Random.nextFloat
			Return nextFloat(Nd4j.order(), shape)
		End Function

		Public Overridable Function nextFloat(ByVal shape() As Long) As INDArray Implements Random.nextFloat
			Return nextFloat(Nd4j.order(), shape)
		End Function

		Public Overridable Function nextFloat(ByVal order As Char, ByVal shape() As Integer) As INDArray Implements Random.nextFloat
			Dim array As INDArray = Nd4j.createUninitialized(shape, order)
			Dim op As New UniformDistribution(array, 0.0, 1.0)
			Nd4j.Executioner.exec(op, Me)

			Return array
		End Function

		Public Overridable Function nextFloat(ByVal order As Char, ByVal shape() As Long) As INDArray Implements Random.nextFloat
			Dim array As INDArray = Nd4j.createUninitialized(shape, order)
			Dim op As New UniformDistribution(array, 0.0, 1.0)
			Nd4j.Executioner.exec(op, Me)

			Return array
		End Function

		Public Overridable Function nextInt(ByVal shape() As Integer) As INDArray Implements Random.nextInt
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Function nextInt(ByVal shape() As Long) As INDArray Implements Random.nextInt
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Function nextInt(ByVal n As Integer, ByVal shape() As Integer) As INDArray Implements Random.nextInt
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Function nextInt(ByVal n As Integer, ByVal shape() As Long) As INDArray Implements Random.nextInt
			Throw New System.NotSupportedException()
		End Function

		''' <summary>
		''' This method returns pointer to RNG state structure.
		''' Please note: DefaultRandom implementation returns NULL here, making it impossible to use with RandomOps
		''' 
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property StatePointer As Pointer Implements Random.getStatePointer
			Get
				Return statePointer_Conflict
			End Get
		End Property

		Public Overridable Sub reSeed() Implements Random.reSeed
			setSeed(DateTimeHelper.CurrentUnixTimeMillis())
		End Sub

		Public Overridable Sub reSeed(ByVal amplifier As Long) Implements Random.reSeed
			Seed = amplifier
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void close() throws Exception
		Public Overrides Sub close()
	'        
	'            Do nothing here, since we use WeakReferences for actual deallocation
	'         
		End Sub

		Public Overridable ReadOnly Property Position As Long Implements Random.getPosition
			Get
				Return currentPosition.get()
			End Get
		End Property
	End Class

End Namespace