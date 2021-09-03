Imports Slf4j = lombok.extern.slf4j.Slf4j
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

Namespace org.deeplearning4j.nn.layers.recurrent

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class FwdPassReturn
	Public Class FwdPassReturn
		'First: needed by standard forward pass only
		Public fwdPassOutput As INDArray
		'Arrays: Needed for backpropGradient only
		Public fwdPassOutputAsArrays() As INDArray
		Public memCellState() As INDArray 'Pre nonlinearity
		Public memCellActivations() As INDArray 'Post nonlinearity
		Public iz() As INDArray
		Public ia() As INDArray
		Public fa() As INDArray
		Public oa() As INDArray
		Public ga() As INDArray
		'Gate pre-outs: only needed if not using sigmoid. For sigmoid: sigmaPrime(z) = sigmoid(z) * (1-sigmoid(z)) -> use activations
		Public fz() As INDArray
		Public oz() As INDArray
		Public gz() As INDArray
		'Next 2: needed for rnnTimeStep only
		Public lastAct As INDArray
		Public lastMemCell As INDArray
		'Last 2: needed only for TBPTT
		Public prevAct As INDArray
		Public prevMemCell As INDArray

		''' <summary>
		''' This method is OPTIONAL, and written mostly for future use
		''' </summary>
		''' <param name="id"> </param>
		Public Overridable Sub leverageTo(ByVal id As String)

			If fwdPassOutput IsNot Nothing Then
				fwdPassOutput = fwdPassOutput.leverageTo(id)
			End If

			If fwdPassOutputAsArrays IsNot Nothing Then
				For i As Integer = 0 To fwdPassOutputAsArrays.Length - 1
					fwdPassOutputAsArrays(i) = fwdPassOutputAsArrays(i).leverageTo(id)
				Next i
			End If

			If memCellState IsNot Nothing Then
				For i As Integer = 0 To memCellState.Length - 1
					memCellState(i) = memCellState(i).leverageTo(id)
				Next i
			End If

			If memCellActivations IsNot Nothing Then
				For i As Integer = 0 To memCellActivations.Length - 1
					memCellActivations(i) = memCellActivations(i).leverageTo(id)
				Next i
			End If

			If fwdPassOutputAsArrays IsNot Nothing Then
				For i As Integer = 0 To fwdPassOutputAsArrays.Length - 1
					fwdPassOutputAsArrays(i) = fwdPassOutputAsArrays(i).leverageTo(id)
				Next i
			End If

			If iz IsNot Nothing Then
				For i As Integer = 0 To iz.Length - 1
					iz(i) = iz(i).leverageTo(id)
				Next i
			End If

			If ia IsNot Nothing Then
				For i As Integer = 0 To ia.Length - 1
					ia(i) = ia(i).leverageTo(id)
				Next i
			End If

			If fa IsNot Nothing Then
				For i As Integer = 0 To fa.Length - 1
					fa(i) = fa(i).leverageTo(id)
				Next i
			End If

			If oa IsNot Nothing Then
				For i As Integer = 0 To oa.Length - 1
					oa(i) = oa(i).leverageTo(id)
				Next i
			End If

			If ga IsNot Nothing Then
				For i As Integer = 0 To ga.Length - 1
					ga(i) = ga(i).leverageTo(id)
				Next i
			End If

			If fz IsNot Nothing Then
				For i As Integer = 0 To fz.Length - 1
					fz(i) = fz(i).leverageTo(id)
				Next i
			End If

			If oz IsNot Nothing Then
				For i As Integer = 0 To oz.Length - 1
					oz(i) = oz(i).leverageTo(id)
				Next i
			End If

			If gz IsNot Nothing Then
				For i As Integer = 0 To gz.Length - 1
					gz(i) = gz(i).leverageTo(id)
				Next i
			End If

			If lastAct IsNot Nothing Then
				lastAct = lastAct.leverageTo(id)
			End If

			If lastMemCell IsNot Nothing Then
				lastMemCell = lastMemCell.leverageTo(id)
			End If

			'Don't want to leverage previous activations if present - assume that has already happened (either passed
			' externally, or was originally a lastAct/lastMemCell)
		End Sub
	End Class

End Namespace