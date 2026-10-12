Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports Loss = org.nd4j.autodiff.listeners.Loss
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
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

Namespace org.nd4j.autodiff.listeners.records

	Public Class LossCurve
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private java.util.List<String> lossNames;
		Private lossNames As IList(Of String)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.nd4j.linalg.api.ndarray.INDArray lossValues;
		Private lossValues As INDArray

		Public Sub New(ByVal losses As IList(Of Loss))
			lossNames = Collections.unmodifiableList(losses(0).getLossNames())
			Dim numLossValues As Integer = losses(0).lossValues().length
			lossValues = Nd4j.create(DataType.FLOAT, losses.Count, losses(0).lossValues().length)

			For i As Integer = 0 To losses.Count - 1
				Dim l As Loss = losses(i)
				Preconditions.checkArgument(l.getLossNames().Equals(lossNames), "Loss names for loss %s differ from others.  Expected %s, got %s", i, lossNames, l.getLossNames())

				Preconditions.checkArgument(l.getLosses().length = numLossValues, "Number of loss values for loss %s differ from others.  Expected %s, got %s", i, numLossValues, l.getLosses().length)

				lossValues = lossValues.putRow(i, Nd4j.createFromArray(l.getLosses()).castTo(DataType.FLOAT))
			Next i
		End Sub

		Public Sub New(ByVal lossValues() As Double, ByVal lossNames As IList(Of String))
			Me.lossValues = Nd4j.createFromArray(New Double()(){ lossValues}).castTo(DataType.FLOAT)
			Me.lossNames = lossNames
		End Sub

		Protected Friend Sub New(ByVal lossValues As INDArray, ByVal lossNames As IList(Of String))
			Preconditions.checkArgument(lossValues.rank() = 2, "lossValues must have a rank of 2, got %s", lossValues.rank())
			Preconditions.checkArgument(lossValues.dataType() = DataType.FLOAT, "lossValues must be type FLOAT, got %s", lossValues.dataType())
			Me.lossValues = lossValues
			Me.lossNames = lossNames
		End Sub

		Public Overridable Function losses() As IList(Of Loss)
'JAVA TO VB CONVERTER NOTE: The local variable losses was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
			Dim losses_Conflict As IList(Of Loss) = New List(Of Loss)()
			Dim i As Integer = 0
			Do While i < lossValues.size(0)
				losses_Conflict.Add(New Loss(lossNames, lossValues.getRow(i).toDoubleVector()))
				i += 1
			Loop
			Return losses_Conflict
		End Function

		''' <summary>
		''' Get the mean loss for a given epoch
		''' 
		''' If epoch is negative, counts backwards from the end.
		''' E.g. losses(-1) gets the last epoch.
		''' </summary>
		''' <param name="epoch"> The epoch to get.  If negative, returns results for the epoch that many epochs from the end </param>
		Public Overridable Function meanLoss(ByVal epoch As Integer) As Loss
			If epoch >= 0 Then
				Return New Loss(lossNames, lossValues.getRow(epoch).toDoubleVector())
			Else
				Return New Loss(lossNames, lossValues.getRow(lossValues.rows() + epoch).toDoubleVector())
			End If
		End Function

		''' <summary>
		''' Get the mean loss for the last epoch.
		''' </summary>
		Public Overridable Function lastMeanLoss() As Loss
			Return meanLoss(-1)
		End Function

		''' <summary>
		''' Return all mean loss values for a given variable
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public float[] meanLoss(@NonNull String lossName)
		Public Overridable Function meanLoss(ByVal lossName As String) As Single()

			Dim idx As Integer = lossNames.IndexOf(lossName)

			Preconditions.checkArgument(idx >= 0, "No loss value for %s.  Existing losses: %s", lossName, lossNames)

			Dim loss(CInt(lossValues.size(0)) - 1) As Single
			Dim i As Integer = 0
			Do While i < lossValues.size(0)
				loss(i) = lossValues.getFloat(i, idx)
				i += 1
			Loop
			Return loss
		End Function

		''' <summary>
		''' Return all mean loss values for a given variable
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public float[] meanLoss(@NonNull SDVariable loss)
		Public Overridable Function meanLoss(ByVal loss As SDVariable) As Single()
			Return meanLoss(loss.name())
		End Function

		''' <summary>
		''' Return the mean loss value for a given variable on a given epoch.
		''' 
		''' See <seealso cref="meanLoss(Integer)"/>
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public float meanLoss(@NonNull String lossName, int epoch)
		Public Overridable Function meanLoss(ByVal lossName As String, ByVal epoch As Integer) As Single

			Dim idx As Integer = lossNames.IndexOf(lossName)

			Preconditions.checkArgument(idx >= 0, "No loss value for %s.  Existing losses: %s", lossName, lossNames)

			If epoch >= 0 Then
				Return lossValues.getFloat(epoch, idx)
			Else
				Return lossValues.getFloat(lossValues.rows() + epoch, idx)
			End If
		End Function

		''' <summary>
		''' Return the mean loss value for a given variable on a given epoch.
		''' 
		''' See <seealso cref="meanLoss(Integer)"/>
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public float meanLoss(@NonNull SDVariable loss, int epoch)
		Public Overridable Function meanLoss(ByVal loss As SDVariable, ByVal epoch As Integer) As Single
			Return meanLoss(loss.name(), epoch)
		End Function

		''' <summary>
		''' Return the mean loss value for a given variable on the last epoch.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public float lastMeanLoss(@NonNull String lossName)
		Public Overridable Function lastMeanLoss(ByVal lossName As String) As Single

			Dim idx As Integer = lossNames.IndexOf(lossName)

			Preconditions.checkArgument(idx >= 0, "No loss value for %s.  Existing losses: %s", lossName, lossNames)

			Return lossValues.getFloat(lossValues.rows() - 1, idx)
		End Function

		''' <summary>
		''' Return the mean loss value for a given variable on the last epoch.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public float lastMeanLoss(@NonNull SDVariable loss)
		Public Overridable Function lastMeanLoss(ByVal loss As SDVariable) As Single
			Return lastMeanLoss(loss.name())
		End Function

		''' <summary>
		''' Return the loss delta between the last epoch and the one before it.
		''' Equivalent to meanLoss(-1) - meanLoss(-2).
		''' A positive delta means the loss is increasing, and a negative delta means it is decreasing.
		''' </summary>
		Public Overridable Function lastMeanDelta() As Loss
			Return lastMeanLoss().sub(meanLoss(-2))
		End Function

		''' <summary>
		''' Return the loss delta between the last epoch and the one before it, for a given variable.
		''' Equivalent to meanLoss(-1) - meanLoss(-2).
		''' A positive delta means the loss is increasing, and a negative delta means it is decreasing.
		''' </summary>
		Public Overridable Function lastMeanDelta(ByVal lossName As String) As Double
			Return lastMeanDelta().getLoss(lossName)
		End Function

		''' <summary>
		''' Return the loss delta between the last epoch and the one before it, for a given variable.
		''' Equivalent to meanLoss(-1) - meanLoss(-2).
		''' A positive delta means the loss is increasing, and a negative delta means it is decreasing.
		''' </summary>
		Public Overridable Function lastMeanDelta(ByVal loss As SDVariable) As Double
			Return lastMeanDelta(loss.name())
		End Function

		''' <summary>
		''' Return a new LossCurve with the given losses added on as the most recent epoch
		''' </summary>
		Public Overridable Function addLossAndCopy(ByVal loss As Loss) As LossCurve
			Return addLossAndCopy(loss.getLosses(), loss.lossNames())
		End Function

		''' <summary>
		''' Return a new LossCurve with the given losses added on as the most recent epoch
		''' </summary>
		Public Overridable Function addLossAndCopy(ByVal values() As Double, ByVal lossNames As IList(Of String)) As LossCurve
			Return New LossCurve(Nd4j.concat(0, lossValues, Nd4j.createFromArray(New Double()(){values}).castTo(DataType.FLOAT)), lossNames)
		End Function
	End Class

End Namespace