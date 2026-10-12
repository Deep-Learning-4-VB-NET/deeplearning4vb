Imports System.Collections.Generic
Imports ArrayUtils = org.apache.commons.lang3.ArrayUtils
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports MultiDataSetPreProcessor = org.nd4j.linalg.dataset.api.MultiDataSetPreProcessor

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

Namespace org.nd4j.linalg.dataset.api.preprocessor.classimbalance


	Public Class UnderSamplingByMaskingMultiDataSetPreProcessor
		Inherits BaseUnderSamplingPreProcessor
		Implements MultiDataSetPreProcessor

		Private targetMinorityDistMap As IDictionary(Of Integer, Double)
		Private minorityLabelMap As IDictionary(Of Integer, Integer) = New Dictionary(Of Integer, Integer)()

		''' <summary>
		''' The target distribution to approximate. Valid values are between (0,0.5].
		''' </summary>
		''' <param name="targetDist"> Key is index of label in multidataset to apply preprocessor. Value is the target dist for that index. </param>
		''' <param name="windowSize"> Usually set to the size of the tbptt </param>
		Public Sub New(ByVal targetDist As IDictionary(Of Integer, Double), ByVal windowSize As Integer)

			For Each index As Integer? In targetDist.Keys
				If targetDist(index) > 0.5 OrElse targetDist(index) <= 0 Then
					Throw New System.ArgumentException("Target distribution for the minority label class has to be greater than 0 and no greater than 0.5. Target distribution of " & targetDist(index) & "given for label at index " & index)
				End If
				minorityLabelMap(index) = 1
			Next index
			Me.targetMinorityDistMap = targetDist
			Me.tbpttWindowSize = windowSize
		End Sub

		''' <summary>
		''' Will change the default minority label from "1" to "0" and correspondingly the majority class from "0" to "1"
		''' for the label at the index specified
		''' </summary>
		Public Overridable Sub overrideMinorityDefault(ByVal index As Integer)
			If targetMinorityDistMap.ContainsKey(index) Then
				minorityLabelMap(index) = 0
			Else
				Throw New System.ArgumentException("Index specified is not contained in the target minority distribution map specified with the preprocessor. Map contains " & ArrayUtils.toString(targetMinorityDistMap.Keys.ToArray()))
			End If
		End Sub

		Public Overridable Sub preProcess(ByVal multiDataSet As MultiDataSet)

			For Each index As Integer? In targetMinorityDistMap.Keys
				Dim label As INDArray = multiDataSet.getLabels(index)
				Dim labelMask As INDArray = multiDataSet.getLabelsMaskArray(index)
				Dim targetMinorityDist As Double = targetMinorityDistMap(index)
				Dim minorityLabel As Integer = minorityLabelMap(index)
				multiDataSet.setLabelsMaskArray(index, adjustMasks(label, labelMask, minorityLabel, targetMinorityDist))
			Next index

		End Sub

	End Class

End Namespace