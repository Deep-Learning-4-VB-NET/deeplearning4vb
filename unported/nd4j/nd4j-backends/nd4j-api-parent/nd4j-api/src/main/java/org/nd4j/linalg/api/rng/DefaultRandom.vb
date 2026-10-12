Imports MersenneTwister = org.apache.commons.math3.random.MersenneTwister
Imports RandomGenerator = org.apache.commons.math3.random.RandomGenerator
Imports SynchronizedRandomGenerator = org.apache.commons.math3.random.SynchronizedRandomGenerator
Imports Pointer = org.bytedeco.javacpp.Pointer
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil

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

Namespace org.nd4j.linalg.api.rng

	Public Class DefaultRandom
		Implements Random, RandomGenerator

'JAVA TO VB CONVERTER NOTE: The field randomGenerator was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend randomGenerator_Conflict As RandomGenerator
'JAVA TO VB CONVERTER NOTE: The field seed was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend seed_Conflict As Long

		''' <summary>
		''' Initialize with a System.currentTimeMillis()
		''' seed
		''' </summary>
		Public Sub New()
			Me.New(DateTimeHelper.CurrentUnixTimeMillis())
		End Sub

		Public Sub New(ByVal seed As Long)
			Me.seed_Conflict = seed
			Me.randomGenerator_Conflict = New SynchronizedRandomGenerator(New MersenneTwister(seed))
		End Sub

		Public Sub New(ByVal randomGenerator As RandomGenerator)
			Me.randomGenerator_Conflict = randomGenerator
		End Sub

		Public Overridable Property Seed Implements Random.setSeed As Integer
			Set(ByVal seed As Integer)
				Me.seed_Conflict = CLng(seed)
				RandomGenerator.setSeed(seed)
			End Set
			Get
				SyncLock Me
					Return Me.seed_Conflict
				End SyncLock
			End Get
		End Property



		Public Overridable WriteOnly Property Seed Implements Random.setSeed As Integer()
			Set(ByVal seed() As Integer)
				Throw New System.NotSupportedException()
			End Set
		End Property

		Public Overridable WriteOnly Property Seed Implements Random.setSeed As Long
			Set(ByVal seed As Long)
				Me.seed_Conflict = seed
				RandomGenerator.setSeed(seed)
			End Set
		End Property

		Public Overridable Sub nextBytes(ByVal bytes() As SByte) Implements Random.nextBytes
			RandomGenerator.nextBytes(bytes)
		End Sub

		Public Overridable Function nextInt() As Integer Implements Random.nextInt
			Return RandomGenerator.nextInt()
		End Function

		Public Overridable Function nextInt(ByVal n As Integer) As Integer Implements Random.nextInt
			Return RandomGenerator.nextInt(n)
		End Function

		Public Overridable Function nextInt(ByVal a As Integer, ByVal n As Integer) As Integer Implements Random.nextInt
			Return nextInt(n - a) + a
		End Function

		Public Overridable Function nextLong() As Long Implements Random.nextLong
			Return RandomGenerator.nextLong()
		End Function

		Public Overridable Function nextBoolean() As Boolean Implements Random.nextBoolean
			Return RandomGenerator.nextBoolean()
		End Function

		Public Overridable Function nextFloat() As Single Implements Random.nextFloat
			Return RandomGenerator.nextFloat()
		End Function

		Public Overridable Function nextDouble() As Double Implements Random.nextDouble
			Return RandomGenerator.nextDouble()
		End Function

		Public Overridable Function nextGaussian() As Double Implements Random.nextGaussian
			Return RandomGenerator.nextGaussian()
		End Function



		Public Overridable Function nextGaussian(ByVal shape() As Long) As INDArray Implements Random.nextGaussian
			Return nextGaussian(Nd4j.order(), shape)
		End Function

		Public Overridable Function nextGaussian(ByVal shape() As Integer) As INDArray Implements Random.nextGaussian
			Return nextGaussian(Nd4j.order(), shape)
		End Function

		Public Overridable Function nextGaussian(ByVal order As Char, ByVal shape() As Integer) As INDArray Implements Random.nextGaussian
			Return nextGaussian(order, ArrayUtil.toLongArray(shape))
		End Function

		Public Overridable Function nextGaussian(ByVal order As Char, ByVal shape() As Long) As INDArray Implements Random.nextGaussian
			Dim length As Long = ArrayUtil.prodLong(shape)
			Dim ret As INDArray = Nd4j.create(shape, order)

			Dim data As DataBuffer = ret.data()
			For i As Long = 0 To length - 1
				data.put(i, nextGaussian())
			Next i

			Return ret
		End Function

		Public Overridable Function nextDouble(ByVal shape() As Long) As INDArray Implements Random.nextDouble
			Return nextDouble(Nd4j.order(), shape)
		End Function

		Public Overridable Function nextDouble(ByVal shape() As Integer) As INDArray Implements Random.nextDouble
			Return nextDouble(Nd4j.order(), shape)
		End Function


		Public Overridable Function nextDouble(ByVal order As Char, ByVal shape() As Integer) As INDArray Implements Random.nextDouble
			Return nextDouble(order, ArrayUtil.toLongArray(shape))
		End Function

		Public Overridable Function nextDouble(ByVal order As Char, ByVal shape() As Long) As INDArray Implements Random.nextDouble
			Dim length As Long = ArrayUtil.prodLong(shape)
			Dim ret As INDArray = Nd4j.create(shape, order)

			Dim data As DataBuffer = ret.data()
			For i As Long = 0 To length - 1
				data.put(i, nextDouble())
			Next i

			Return ret
		End Function

		Public Overridable Function nextFloat(ByVal shape() As Long) As INDArray Implements Random.nextFloat
			Return nextFloat(Nd4j.order(), shape)
		End Function

		Public Overridable Function nextFloat(ByVal shape() As Integer) As INDArray Implements Random.nextFloat
			Return nextFloat(Nd4j.order(), shape)
		End Function

		Public Overridable Function nextFloat(ByVal order As Char, ByVal shape() As Integer) As INDArray Implements Random.nextFloat
			Return nextFloat(order, ArrayUtil.toLongArray(shape))
		End Function

		Public Overridable Function nextFloat(ByVal order As Char, ByVal shape() As Long) As INDArray Implements Random.nextFloat
			Dim length As Long = ArrayUtil.prodLong(shape)
			Dim ret As INDArray = Nd4j.create(shape, order)

			Dim data As DataBuffer = ret.data()
			For i As Long = 0 To length - 1
				data.put(i, nextFloat())
			Next i

			Return ret
		End Function

		Public Overridable Function nextInt(ByVal shape() As Integer) As INDArray Implements Random.nextInt
			Return nextInt(ArrayUtil.toLongArray(shape))
		End Function

		Public Overridable Function nextInt(ByVal shape() As Long) As INDArray Implements Random.nextInt
			Dim length As Integer = ArrayUtil.prod(shape)
			Dim ret As INDArray = Nd4j.create(shape)

			Dim data As DataBuffer = ret.data()
			For i As Integer = 0 To length - 1
				data.put(i, nextInt())
			Next i

			Return ret
		End Function

		Public Overridable Function nextInt(ByVal n As Integer, ByVal shape() As Integer) As INDArray Implements Random.nextInt
			Return nextInt(n, ArrayUtil.toLongArray(shape))
		End Function


		Public Overridable Function nextInt(ByVal n As Integer, ByVal shape() As Long) As INDArray Implements Random.nextInt
			Dim length As Integer = ArrayUtil.prod(shape)
			Dim ret As INDArray = Nd4j.create(shape)

			Dim data As DataBuffer = ret.data()
			For i As Integer = 0 To length - 1
				data.put(i, nextInt(n))
			Next i

			Return ret
		End Function


		Public Overridable ReadOnly Property RandomGenerator As RandomGenerator
			Get
				SyncLock Me
					Return randomGenerator_Conflict
				End SyncLock
			End Get
		End Property



		''' <summary>
		''' This method returns pointer to RNG state structure.
		''' Please note: DefaultRandom implementation returns NULL here, making it impossible to use with RandomOps
		''' 
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property StatePointer As Pointer Implements Random.getStatePointer
			Get
				Return Nothing
			End Get
		End Property

		Public Overridable ReadOnly Property Position As Long Implements Random.getPosition
			Get
				Return 0
			End Get
		End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void close() throws Exception
		Public Overrides Sub close()
			'
		End Sub

		''' <summary>
		''' Identical to setSeed(System.currentTimeMillis());
		''' </summary>
		Public Overridable Sub reSeed() Implements Random.reSeed
			reSeed(DateTimeHelper.CurrentUnixTimeMillis())
		End Sub

		''' <summary>
		''' Identical to setSeed(seed);
		''' </summary>
		''' <param name="seed"> </param>
		Public Overridable Sub reSeed(ByVal seed As Long) Implements Random.reSeed
			Me.Seed = seed
		End Sub

		Public Overridable Function rootState() As Long Implements Random.rootState
			Return 0L
		End Function

		Public Overridable Function nodeState() As Long Implements Random.nodeState
			Return 0L
		End Function

		Public Overridable Sub setStates(ByVal rootState As Long, ByVal nodeState As Long) Implements Random.setStates
			'no-op
		End Sub
	End Class

End Namespace