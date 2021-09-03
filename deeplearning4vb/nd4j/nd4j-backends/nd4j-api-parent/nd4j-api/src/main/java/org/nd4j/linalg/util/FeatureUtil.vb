Imports val = lombok.val
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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

Namespace org.nd4j.linalg.util

	Public Class FeatureUtil
		''' <summary>
		''' Creates an out come vector from the specified inputs
		''' </summary>
		''' <param name="index">       the index of the label </param>
		''' <param name="numOutcomes"> the number of possible outcomes </param>
		''' <returns> a binary label matrix used for supervised learning </returns>
		Public Shared Function toOutcomeVector(ByVal index As Long, ByVal numOutcomes As Long) As INDArray
			If index > Integer.MaxValue OrElse numOutcomes > Integer.MaxValue Then
				Throw New System.NotSupportedException()
			End If

			Dim nums As val = New Integer(CInt(numOutcomes) - 1){}
			nums(CInt(index)) = 1
			Return NDArrayUtil.toNDArray(nums)
		End Function


		''' <summary>
		''' Creates an out come vector from the specified inputs
		''' </summary>
		''' <param name="index">       the index of the label </param>
		''' <param name="numOutcomes"> the number of possible outcomes </param>
		''' <returns> a binary label matrix used for supervised learning </returns>
		Public Shared Function toOutcomeMatrix(ByVal index() As Integer, ByVal numOutcomes As Long) As INDArray
			Dim ret As INDArray = Nd4j.create(index.Length, numOutcomes)
			Dim i As Integer = 0
			Do While i < ret.rows()
				Dim nums(CInt(numOutcomes) - 1) As Integer
				nums(index(i)) = 1
				ret.putRow(i, NDArrayUtil.toNDArray(nums))
				i += 1
			Loop

			Return ret
		End Function

		Public Shared Sub normalizeMatrix(ByVal toNormalize As INDArray)
			Dim columnMeans As INDArray = toNormalize.mean(0)
			toNormalize.subiRowVector(columnMeans)
			Dim std As INDArray = toNormalize.std(0)
			std.addi(Nd4j.scalar(1e-12))
			toNormalize.diviRowVector(std)
		End Sub

		''' <summary>
		''' Divides each row by its max
		''' </summary>
		''' <param name="toScale"> the matrix to divide by its row maxes </param>
		Public Shared Sub scaleByMax(ByVal toScale As INDArray)
			Dim scale As INDArray = toScale.max(1)
			Dim i As Integer = 0
			Do While i < toScale.rows()
				Dim scaleBy As Double = scale.getDouble(i)
				toScale.putRow(i, toScale.getRow(i).divi(scaleBy))
				i += 1
			Loop
		End Sub


		''' <summary>
		''' Scales the ndarray columns
		''' to the given min/max values
		''' </summary>
		''' <param name="min"> the minimum number </param>
		''' <param name="max"> the max number </param>
		Public Shared Sub scaleMinMax(ByVal min As Double, ByVal max As Double, ByVal toScale As INDArray)
			'X_std = (X - X.min(axis=0)) / (X.max(axis=0) - X.min(axis=0)) X_scaled = X_std * (max - min) + min

			Dim min2 As INDArray = toScale.min(0)
			Dim max2 As INDArray = toScale.max(0)

			Dim std As INDArray = toScale.subRowVector(min2).diviRowVector(max2.sub(min2))

			Dim scaled As INDArray = std.mul(max - min).addi(min)
			toScale.assign(scaled)
		End Sub


	End Class

End Namespace