Imports System
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.api.DataSet
Imports DataSetPreProcessor = org.nd4j.linalg.dataset.api.DataSetPreProcessor

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

Namespace org.nd4j.linalg.dataset.api.preprocessor

	<Serializable>
	Public Class PermuteDataSetPreProcessor
		Implements DataSetPreProcessor

		Private ReadOnly permutationType As PermutationTypes
		Private ReadOnly rearrange() As Integer

		Public Enum PermutationTypes
			NCHWtoNHWC
			NHWCtoNCHW
			Custom

		End Enum
		Public Sub New(ByVal permutationType As PermutationTypes)
			Preconditions.checkArgument(permutationType <> PermutationTypes.Custom, "Use the ctor PermuteDataSetPreProcessor(int... rearrange) for custom permutations.")

			Me.permutationType = permutationType
			rearrange = Nothing
		End Sub

		''' <param name="rearrange"> The new order. For example PermuteDataSetPreProcessor(1, 2, 0) will rearrange the middle dimension first, the last one in the middle and the first one last. </param>
		Public Sub New(ParamArray ByVal rearrange() As Integer)

			Me.permutationType = PermutationTypes.Custom
			Me.rearrange = rearrange
		End Sub

		Public Overridable Sub preProcess(ByVal dataSet As DataSet)
			Preconditions.checkNotNull(dataSet, "Encountered null dataSet")

			If dataSet.Empty Then
				Return
			End If

			Dim input As INDArray = dataSet.Features
			Dim output As INDArray
			Select Case permutationType
				Case org.nd4j.linalg.dataset.api.preprocessor.PermuteDataSetPreProcessor.PermutationTypes.NCHWtoNHWC
					output = input.permute(0, 2, 3, 1)

				Case org.nd4j.linalg.dataset.api.preprocessor.PermuteDataSetPreProcessor.PermutationTypes.NHWCtoNCHW
					output = input.permute(0, 3, 1, 2)

				Case org.nd4j.linalg.dataset.api.preprocessor.PermuteDataSetPreProcessor.PermutationTypes.Custom
					output = input.permute(rearrange)

				Case Else
					output = input
			End Select

			dataSet.Features = output
		End Sub
	End Class

End Namespace