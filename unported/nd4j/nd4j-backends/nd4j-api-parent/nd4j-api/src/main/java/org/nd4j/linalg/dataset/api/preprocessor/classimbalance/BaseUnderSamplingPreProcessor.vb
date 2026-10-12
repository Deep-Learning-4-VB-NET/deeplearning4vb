Imports System
Imports Microsoft.VisualBasic
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BernoulliDistribution = org.nd4j.linalg.api.ops.random.impl.BernoulliDistribution
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports BooleanIndexing = org.nd4j.linalg.indexing.BooleanIndexing
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports Conditions = org.nd4j.linalg.indexing.conditions.Conditions

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

	Public MustInherit Class BaseUnderSamplingPreProcessor

		Protected Friend tbpttWindowSize As Integer
		Private maskAllMajorityWindows As Boolean = True
'JAVA TO VB CONVERTER NOTE: The field donotMaskMinorityWindows was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private donotMaskMinorityWindows_Conflict As Boolean = False

		''' <summary>
		''' If a tbptt segment is all majority class labels default behaviour is to mask all time steps in the segment.
		''' donotMaskAllMajorityWindows() will override the default and mask (1-targetDist)% of the time steps
		''' </summary>
		Public Overridable Sub donotMaskAllMajorityWindows()
			Me.maskAllMajorityWindows = False
		End Sub

		''' <summary>
		''' If set will not mask timesteps if they fall in a tbptt segment with at least one minority class label
		''' </summary>
		Public Overridable Sub donotMaskMinorityWindows()
			Me.donotMaskMinorityWindows_Conflict = True
		End Sub

		Public Overridable Function adjustMasks(ByVal label As INDArray, ByVal labelMask As INDArray, ByVal minorityLabel As Integer, ByVal targetDist As Double) As INDArray

			If labelMask Is Nothing Then
				labelMask = Nd4j.ones(label.size(0), label.size(2))
			End If
			validateData(label, labelMask)

			Dim bernoullis As INDArray = Nd4j.zeros(labelMask.shape())
			Dim currentTimeSliceEnd As Long = label.size(2)
			'iterate over each tbptt window
			Do While currentTimeSliceEnd > 0

				Dim currentTimeSliceStart As Long = Math.Max(currentTimeSliceEnd - tbpttWindowSize, 0)

				'get views for current time slice
				Dim currentWindowBernoulli As INDArray = bernoullis.get(NDArrayIndex.all(), NDArrayIndex.interval(currentTimeSliceStart, currentTimeSliceEnd))
				Dim currentMask As INDArray = labelMask.get(NDArrayIndex.all(), NDArrayIndex.interval(currentTimeSliceStart, currentTimeSliceEnd))
				Dim currentLabel As INDArray
				If label.size(1) = 2 Then
					'if one hot grab the right index
					currentLabel = label.get(NDArrayIndex.all(), NDArrayIndex.point(minorityLabel), NDArrayIndex.interval(currentTimeSliceStart, currentTimeSliceEnd))
				Else
					currentLabel = label.get(NDArrayIndex.all(), NDArrayIndex.point(0), NDArrayIndex.interval(currentTimeSliceStart, currentTimeSliceEnd))
					If minorityLabel = 0 Then
						currentLabel = currentLabel.rsub(1.0) 'rsub(1.0) is equivalent to swapping 0s and 1s
					End If
				End If

				'calculate required probabilities and write into the view
				currentWindowBernoulli.assign(calculateBernoulli(currentLabel, currentMask, targetDist))

				currentTimeSliceEnd = currentTimeSliceStart
			Loop

			Return Nd4j.Executioner.exec(New BernoulliDistribution(Nd4j.createUninitialized(bernoullis.shape()), bernoullis), Nd4j.Random)
		End Function

	'    
	'    Given a list of labels return the bernoulli prob that the masks
	'    will be sampled at to meet the target minority label distribution
	'    
	'    Masks at time steps where label is the minority class will always be one
	'        i.e a bernoulli with p = 1
	'    Masks at time steps where label is the majority class will be sampled from
	'        a bernoulli dist with p
	'            = (minorityCount/majorityCount) * ((1-targetDist)/targetDist)
	'    
		Private Function calculateBernoulli(ByVal minorityLabels As INDArray, ByVal labelMask As INDArray, ByVal targetMinorityDist As Double) As INDArray

			Dim minorityClass As INDArray = minorityLabels.castTo(Nd4j.defaultFloatingPointType()).muli(labelMask)
			Dim majorityClass As INDArray = minorityLabels.rsub(1.0).muli(labelMask) 'rsub(1.0) is equivalent to swapping 0s and 1s

			'all minorityLabel class, keep masks as is
			'presence of minoriy class and donotmask minority windows set to true return label as is
			If majorityClass.sumNumber().intValue() = 0 OrElse (minorityClass.sumNumber().intValue() > 0 AndAlso donotMaskMinorityWindows_Conflict) Then
				Return labelMask
			End If
			'all majority class and set to not mask all majority windows sample majority class by 1-targetMinorityDist
			If minorityClass.sumNumber().intValue() = 0 AndAlso Not maskAllMajorityWindows Then
				Return labelMask.muli(1 - targetMinorityDist)
			End If

			'Probabilities to be used for bernoulli sampling
			Dim minoritymajorityRatio As INDArray = minorityClass.sum(1).div(majorityClass.sum(1))
			Dim majorityBernoulliP As INDArray = minoritymajorityRatio.muli(1 - targetMinorityDist).divi(targetMinorityDist)
			BooleanIndexing.replaceWhere(majorityBernoulliP, 1.0, Conditions.greaterThan(1.0)) 'if minority ratio is already met round down to 1.0
			Return majorityClass.muliColumnVector(majorityBernoulliP).addi(minorityClass)
		End Function

		Private Sub validateData(ByVal label As INDArray, ByVal labelMask As INDArray)
			If label.rank() <> 3 Then
				Throw New System.ArgumentException("UnderSamplingByMaskingPreProcessor can only be applied to a time series dataset")
			End If
			If label.size(1) > 2 Then
				Throw New System.ArgumentException("UnderSamplingByMaskingPreProcessor can only be applied to labels that represent binary classes. Label size was found to be " & label.size(1) & ".Expecting size=1 or size=2.")
			End If
			If label.size(1) = 2 Then
				'check if label is of size one hot
				Dim sum1 As INDArray = label.sum(1).mul(labelMask)
				Dim floatMask As INDArray = labelMask.castTo(label.dataType())
				If Not sum1.Equals(floatMask) Then
					Throw New System.ArgumentException("Labels of size minibatchx2xtimesteps are expected to be one hot." & label.ToString() & vbLf & " is not one-hot")
				End If
			End If
		End Sub

	End Class

End Namespace