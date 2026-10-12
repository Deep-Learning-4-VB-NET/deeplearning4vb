Imports System.Collections.Generic
Imports AccessLevel = lombok.AccessLevel
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports Setter = lombok.Setter
Imports History = org.nd4j.autodiff.listeners.records.History
Imports Listener = org.nd4j.autodiff.listeners.Listener
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports TrainingConfig = org.nd4j.autodiff.samediff.TrainingConfig
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports MultiDataSetIteratorAdapter = org.nd4j.linalg.dataset.adapter.MultiDataSetIteratorAdapter
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports MultiDataSetIterator = org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator

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

Namespace org.nd4j.autodiff.samediff.config


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter public class FitConfig
	Public Class FitConfig
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter(lombok.AccessLevel.NONE) private org.nd4j.autodiff.samediff.SameDiff sd;
		Private sd As SameDiff

		Private trainingData As MultiDataSetIterator

		Private validationData As MultiDataSetIterator = Nothing

'JAVA TO VB CONVERTER NOTE: The field epochs was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private epochs_Conflict As Integer = -1

'JAVA TO VB CONVERTER NOTE: The field validationFrequency was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private validationFrequency_Conflict As Integer = 1

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NonNull private java.util.List<org.nd4j.autodiff.listeners.Listener> listeners = new java.util.ArrayList<>();
'JAVA TO VB CONVERTER NOTE: The field listeners was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private listeners_Conflict As IList(Of Listener) = New List(Of Listener)()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public FitConfig(@NonNull SameDiff sd)
		Public Sub New(ByVal sd As SameDiff)
			Me.sd = sd
		End Sub

		''' <summary>
		''' Set the number of epochs to train for
		''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter epochs was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
		Public Overridable Function epochs(ByVal epochs_Conflict As Integer) As FitConfig
			Me.epochs_Conflict = epochs_Conflict
			Return Me
		End Function

		''' <summary>
		''' Set the training data
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public FitConfig train(@NonNull MultiDataSetIterator trainingData)
		Public Overridable Function train(ByVal trainingData As MultiDataSetIterator) As FitConfig
			Me.trainingData = trainingData
			Return Me
		End Function

		''' <summary>
		''' Set the training data
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public FitConfig train(@NonNull DataSetIterator trainingData)
		Public Overridable Function train(ByVal trainingData As DataSetIterator) As FitConfig
			Return train(New MultiDataSetIteratorAdapter(trainingData))
		End Function

		''' <summary>
		''' Set the training data and number of epochs
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public FitConfig train(@NonNull MultiDataSetIterator trainingData, int epochs)
		Public Overridable Function train(ByVal trainingData As MultiDataSetIterator, ByVal epochs As Integer) As FitConfig
			Return train(trainingData).epochs(epochs)
		End Function

		''' <summary>
		''' Set the training data and number of epochs
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public FitConfig train(@NonNull DataSetIterator trainingData, int epochs)
		Public Overridable Function train(ByVal trainingData As DataSetIterator, ByVal epochs As Integer) As FitConfig
			Return train(trainingData).epochs(epochs)
		End Function

		''' <summary>
		''' Set the validation data
		''' </summary>
		Public Overridable Function validate(ByVal validationData As MultiDataSetIterator) As FitConfig
			Me.validationData = validationData
			Return Me
		End Function

		''' <summary>
		''' Set the validation data
		''' </summary>
		Public Overridable Function validate(ByVal validationData As DataSetIterator) As FitConfig
			If validationData Is Nothing Then
				Return validate(DirectCast(Nothing, MultiDataSetIterator))
			Else
				Return validate(New MultiDataSetIteratorAdapter(validationData))
			End If
		End Function

		''' <summary>
		''' Set the validation frequency.  Validation will be preformed once every so many epochs.
		''' <para>
		''' Specifically, validation will be preformed when i % validationFrequency == 0
		''' </para>
		''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter validationFrequency was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
		Public Overridable Function validationFrequency(ByVal validationFrequency_Conflict As Integer) As FitConfig
			Me.validationFrequency_Conflict = validationFrequency_Conflict
			Return Me
		End Function

		''' <summary>
		''' Set the validation data and frequency
		''' <para>
		''' Specifically, validation will be preformed when i % validationFrequency == 0
		''' </para>
		''' </summary>
		Public Overridable Function validate(ByVal validationData As MultiDataSetIterator, ByVal validationFrequency As Integer) As FitConfig
			Return validate(validationData).validationFrequency(validationFrequency)
		End Function

		''' <summary>
		''' Set the validation data and frequency
		''' <para>
		''' Specifically, validation will be preformed when i % validationFrequency == 0
		''' </para>
		''' </summary>
		Public Overridable Function validate(ByVal validationData As DataSetIterator, ByVal validationFrequency As Integer) As FitConfig
			Return validate(validationData).validationFrequency(validationFrequency)
		End Function

		''' <summary>
		''' Add listeners for this operation
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public FitConfig listeners(@NonNull Listener... listeners)
'JAVA TO VB CONVERTER NOTE: The parameter listeners was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
		Public Overridable Function listeners(ParamArray ByVal listeners_Conflict() As Listener) As FitConfig
			CType(Me.listeners_Conflict, List(Of Listener)).AddRange(New List(Of Listener) From {listeners_Conflict})
			Return Me
		End Function


		Private Sub validateConfig()
			Preconditions.checkNotNull(trainingData, "Training data must not be null")
			Preconditions.checkState(epochs_Conflict > 0, "Epochs must be > 0, got %s", epochs_Conflict)

			If validationData IsNot Nothing Then
				Preconditions.checkState(validationFrequency_Conflict > 0, "Validation Frequency must be > 0 if validation data is given, got %s", validationFrequency_Conflict)
			End If
		End Sub

		''' <summary>
		''' Do the training.
		''' </summary>
		''' <returns> a <seealso cref="History"/> object containing the history information for this training operation
		''' (evaluations specified in the <seealso cref="TrainingConfig"/>, loss values, and timing information). </returns>
		Public Overridable Function exec() As History
			validateConfig()

			Return sd.fit(trainingData, epochs_Conflict, validationData, validationFrequency_Conflict, CType(listeners_Conflict, List(Of Listener)).ToArray())
		End Function

	End Class

End Namespace