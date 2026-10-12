Imports System.Collections.Generic
Imports AccessLevel = lombok.AccessLevel
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports Setter = lombok.Setter
Imports Listener = org.nd4j.autodiff.listeners.Listener
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports SameDiffUtils = org.nd4j.autodiff.util.SameDiffUtils
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports MultiDataSetIteratorAdapter = org.nd4j.linalg.dataset.adapter.MultiDataSetIteratorAdapter
Imports SingletonMultiDataSetIterator = org.nd4j.linalg.dataset.adapter.SingletonMultiDataSetIterator
Imports DataSet = org.nd4j.linalg.dataset.api.DataSet
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
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
'ORIGINAL LINE: @Getter @Setter public class OutputConfig
	Public Class OutputConfig
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter(lombok.AccessLevel.NONE) private org.nd4j.autodiff.samediff.SameDiff sd;
		Private sd As SameDiff

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NonNull private java.util.List<String> outputs = new java.util.ArrayList<>();
		Private outputs As IList(Of String) = New List(Of String)()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NonNull private java.util.List<org.nd4j.autodiff.listeners.Listener> listeners = new java.util.ArrayList<>();
'JAVA TO VB CONVERTER NOTE: The field listeners was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private listeners_Conflict As IList(Of Listener) = New List(Of Listener)()

'JAVA TO VB CONVERTER NOTE: The field data was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private data_Conflict As MultiDataSetIterator

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public OutputConfig(@NonNull SameDiff sd)
		Public Sub New(ByVal sd As SameDiff)
			Me.sd = sd
		End Sub

		''' <summary>
		''' Add required outputs
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public OutputConfig output(@NonNull String... outputs)
		Public Overridable Function output(ParamArray ByVal outputs() As String) As OutputConfig
			CType(Me.outputs, List(Of String)).AddRange(New List(Of String) From {outputs})
			Return Me
		End Function

		''' <summary>
		''' Add required outputs
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public OutputConfig output(@NonNull SDVariable... outputs)
		Public Overridable Function output(ParamArray ByVal outputs() As SDVariable) As OutputConfig
			Dim outNames(outputs.Length - 1) As String
			For i As Integer = 0 To outputs.Length - 1
				outNames(i) = outputs(i).name()
			Next i

			Return output(outNames)
		End Function

		''' <summary>
		''' Set the data to use as input.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public OutputConfig data(@NonNull MultiDataSetIterator data)
'JAVA TO VB CONVERTER NOTE: The parameter data was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
		Public Overridable Function data(ByVal data_Conflict As MultiDataSetIterator) As OutputConfig
			Me.data_Conflict = data_Conflict
			Return Me
		End Function

		''' <summary>
		''' Set the data to use as input.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public OutputConfig data(@NonNull DataSetIterator data)
'JAVA TO VB CONVERTER NOTE: The parameter data was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
		Public Overridable Function data(ByVal data_Conflict As DataSetIterator) As OutputConfig
			Me.data_Conflict = New MultiDataSetIteratorAdapter(data_Conflict)
			Return Me
		End Function

		''' <summary>
		''' Set the data to use as input.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public OutputConfig data(@NonNull DataSet data)
'JAVA TO VB CONVERTER NOTE: The parameter data was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
		Public Overridable Function data(ByVal data_Conflict As DataSet) As OutputConfig
			Return data(New SingletonMultiDataSetIterator(data_Conflict.toMultiDataSet()))
		End Function

		''' <summary>
		''' Set the data to use as input.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public OutputConfig data(@NonNull MultiDataSet data)
'JAVA TO VB CONVERTER NOTE: The parameter data was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
		Public Overridable Function data(ByVal data_Conflict As MultiDataSet) As OutputConfig
			Return data(New SingletonMultiDataSetIterator(data_Conflict))
		End Function

		''' <summary>
		''' Add listeners for this operation
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public OutputConfig listeners(@NonNull Listener... listeners)
'JAVA TO VB CONVERTER NOTE: The parameter listeners was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
		Public Overridable Function listeners(ParamArray ByVal listeners_Conflict() As Listener) As OutputConfig
			CType(Me.listeners_Conflict, List(Of Listener)).AddRange(New List(Of Listener) From {listeners_Conflict})
			Return Me
		End Function

		Private Sub validateConfig()
			Preconditions.checkNotNull(data_Conflict, "Must specify data.  It may not be null.")
		End Sub

		''' <summary>
		''' Do inference and return the results.
		''' 
		''' Uses concatenation on the outputs of <seealso cref="execBatches()"/> which may cause issues with some inputs. RNNs with
		''' variable time series length and CNNs with variable image sizes will most likely have issues.
		''' </summary>
		Public Overridable Function exec() As IDictionary(Of String, INDArray)
			Return sd.output(data_Conflict, listeners_Conflict, CType(outputs, List(Of String)).ToArray())
		End Function

		''' <summary>
		''' Do inference and return the results in batches.
		''' </summary>
		Public Overridable Function execBatches() As IList(Of IDictionary(Of String, INDArray))
			Return sd.outputBatches(data_Conflict, listeners_Conflict, CType(outputs, List(Of String)).ToArray())
		End Function

		''' <summary>
		''' Do inference and return the results for the single output variable specified.
		''' 
		''' Only works if exactly one output is specified.
		''' 
		''' Uses concatenation on the outputs of <seealso cref="execBatches()"/> which may cause issues with some inputs. RNNs with
		''' variable time series length and CNNs with variable image sizes will most likely have issues.
		''' </summary>
		Public Overridable Function execSingle() As INDArray
			Preconditions.checkState(outputs.Count = 1, "Can only use execSingle() when exactly one output is specified, there were %s", outputs.Count)

			Return sd.output(data_Conflict, listeners_Conflict, CType(outputs, List(Of String)).ToArray())(outputs(0))
		End Function


		''' <summary>
		''' Do inference and return the results (in batches) for the single output variable specified.
		''' 
		''' Only works if exactly one output is specified.
		''' </summary>
		Public Overridable Function execSingleBatches() As IList(Of INDArray)
			Preconditions.checkState(outputs.Count = 1, "Can only use execSingleBatches() when exactly one output is specified, there were %s", outputs.Count)

			Return SameDiffUtils.getSingleOutput(sd.outputBatches(data_Conflict, listeners_Conflict, CType(outputs, List(Of String)).ToArray()), outputs(0))
		End Function
	End Class

End Namespace