Imports Pointer = org.bytedeco.javacpp.Pointer
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray

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

Namespace org.deeplearning4j.rl4j.support

	Public Class MockRandom
		Implements org.nd4j.linalg.api.rng.Random

		Private randomDoubleValuesIdx As Integer = 0
		Private ReadOnly randomDoubleValues() As Double

		Private randomIntValuesIdx As Integer = 0
		Private ReadOnly randomIntValues() As Integer

		Public Sub New(ByVal randomDoubleValues() As Double, ByVal randomIntValues() As Integer)
			Me.randomDoubleValues = randomDoubleValues
			Me.randomIntValues = randomIntValues
		End Sub

		Public Overridable Property Seed Implements org.nd4j.linalg.api.rng.Random.setSeed As Integer
			Set(ByVal i As Integer)
    
			End Set
			Get
				Return 0
			End Get
		End Property

		Public Overridable WriteOnly Property Seed Implements org.nd4j.linalg.api.rng.Random.setSeed As Integer()
			Set(ByVal ints() As Integer)
    
			End Set
		End Property

		Public Overridable WriteOnly Property Seed Implements org.nd4j.linalg.api.rng.Random.setSeed As Long
			Set(ByVal l As Long)
    
			End Set
		End Property


		Public Overridable Sub nextBytes(ByVal bytes() As SByte) Implements org.nd4j.linalg.api.rng.Random.nextBytes

		End Sub

		Public Overridable Function nextInt() As Integer Implements org.nd4j.linalg.api.rng.Random.nextInt
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: return randomIntValues[randomIntValuesIdx++];
			Dim tempVar = randomIntValues(randomIntValuesIdx)
				randomIntValuesIdx += 1
				Return tempVar
		End Function

		Public Overridable Function nextInt(ByVal i As Integer) As Integer Implements org.nd4j.linalg.api.rng.Random.nextInt
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: return randomIntValues[randomIntValuesIdx++];
			Dim tempVar = randomIntValues(randomIntValuesIdx)
				randomIntValuesIdx += 1
				Return tempVar
		End Function

		Public Overridable Function nextInt(ByVal i As Integer, ByVal i1 As Integer) As Integer Implements org.nd4j.linalg.api.rng.Random.nextInt
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: return randomIntValues[randomIntValuesIdx++];
			Dim tempVar = randomIntValues(randomIntValuesIdx)
				randomIntValuesIdx += 1
				Return tempVar
		End Function

		Public Overridable Function nextLong() As Long Implements org.nd4j.linalg.api.rng.Random.nextLong
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: return randomIntValues[randomIntValuesIdx++];
			Dim tempVar = randomIntValues(randomIntValuesIdx)
				randomIntValuesIdx += 1
				Return tempVar
		End Function

		Public Overridable Function nextBoolean() As Boolean Implements org.nd4j.linalg.api.rng.Random.nextBoolean
			Return False
		End Function

		Public Overridable Function nextFloat() As Single Implements org.nd4j.linalg.api.rng.Random.nextFloat
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: return (float)randomDoubleValues[randomDoubleValuesIdx++];
			Dim tempVar = CSng(randomDoubleValues(randomDoubleValuesIdx))
				randomDoubleValuesIdx += 1
				Return tempVar
		End Function

		Public Overridable Function nextDouble() As Double Implements org.nd4j.linalg.api.rng.Random.nextDouble
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: return randomDoubleValues[randomDoubleValuesIdx++];
			Dim tempVar = randomDoubleValues(randomDoubleValuesIdx)
				randomDoubleValuesIdx += 1
				Return tempVar
		End Function

		Public Overridable Function nextGaussian() As Double Implements org.nd4j.linalg.api.rng.Random.nextGaussian
			Return 0
		End Function

		Public Overridable Function nextGaussian(ByVal ints() As Integer) As INDArray Implements org.nd4j.linalg.api.rng.Random.nextGaussian
			Return Nothing
		End Function

		Public Overridable Function nextGaussian(ByVal longs() As Long) As INDArray Implements org.nd4j.linalg.api.rng.Random.nextGaussian
			Return Nothing
		End Function

		Public Overridable Function nextGaussian(ByVal c As Char, ByVal ints() As Integer) As INDArray Implements org.nd4j.linalg.api.rng.Random.nextGaussian
			Return Nothing
		End Function

		Public Overridable Function nextGaussian(ByVal c As Char, ByVal longs() As Long) As INDArray Implements org.nd4j.linalg.api.rng.Random.nextGaussian
			Return Nothing
		End Function

		Public Overridable Function nextDouble(ByVal ints() As Integer) As INDArray Implements org.nd4j.linalg.api.rng.Random.nextDouble
			Return Nothing
		End Function

		Public Overridable Function nextDouble(ByVal longs() As Long) As INDArray Implements org.nd4j.linalg.api.rng.Random.nextDouble
			Return Nothing
		End Function

		Public Overridable Function nextDouble(ByVal c As Char, ByVal ints() As Integer) As INDArray Implements org.nd4j.linalg.api.rng.Random.nextDouble
			Return Nothing
		End Function

		Public Overridable Function nextDouble(ByVal c As Char, ByVal longs() As Long) As INDArray Implements org.nd4j.linalg.api.rng.Random.nextDouble
			Return Nothing
		End Function

		Public Overridable Function nextFloat(ByVal ints() As Integer) As INDArray Implements org.nd4j.linalg.api.rng.Random.nextFloat
			Return Nothing
		End Function

		Public Overridable Function nextFloat(ByVal longs() As Long) As INDArray Implements org.nd4j.linalg.api.rng.Random.nextFloat
			Return Nothing
		End Function

		Public Overridable Function nextFloat(ByVal c As Char, ByVal ints() As Integer) As INDArray Implements org.nd4j.linalg.api.rng.Random.nextFloat
			Return Nothing
		End Function

		Public Overridable Function nextFloat(ByVal c As Char, ByVal longs() As Long) As INDArray Implements org.nd4j.linalg.api.rng.Random.nextFloat
			Return Nothing
		End Function

		Public Overridable Function nextInt(ByVal ints() As Integer) As INDArray Implements org.nd4j.linalg.api.rng.Random.nextInt
			Return Nothing
		End Function

		Public Overridable Function nextInt(ByVal longs() As Long) As INDArray Implements org.nd4j.linalg.api.rng.Random.nextInt
			Return Nothing
		End Function

		Public Overridable Function nextInt(ByVal i As Integer, ByVal ints() As Integer) As INDArray Implements org.nd4j.linalg.api.rng.Random.nextInt
			Return Nothing
		End Function

		Public Overridable Function nextInt(ByVal i As Integer, ByVal longs() As Long) As INDArray Implements org.nd4j.linalg.api.rng.Random.nextInt
			Return Nothing
		End Function

		Public Overridable ReadOnly Property StatePointer As Pointer Implements org.nd4j.linalg.api.rng.Random.getStatePointer
			Get
				Return Nothing
			End Get
		End Property

		Public Overridable ReadOnly Property Position As Long Implements org.nd4j.linalg.api.rng.Random.getPosition
			Get
				Return 0
			End Get
		End Property

		Public Overridable Sub reSeed() Implements org.nd4j.linalg.api.rng.Random.reSeed

		End Sub

		Public Overridable Sub reSeed(ByVal l As Long) Implements org.nd4j.linalg.api.rng.Random.reSeed

		End Sub

		Public Overridable Function rootState() As Long Implements org.nd4j.linalg.api.rng.Random.rootState
			Return 0
		End Function

		Public Overridable Function nodeState() As Long Implements org.nd4j.linalg.api.rng.Random.nodeState
			Return 0
		End Function

		Public Overridable Sub setStates(ByVal l As Long, ByVal l1 As Long) Implements org.nd4j.linalg.api.rng.Random.setStates

		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void close() throws Exception
		Public Overrides Sub close()

		End Sub
	End Class

End Namespace