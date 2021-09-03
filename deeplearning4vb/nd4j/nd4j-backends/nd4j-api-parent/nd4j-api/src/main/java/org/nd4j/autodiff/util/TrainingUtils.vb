Imports System
Imports System.Collections.Generic
Imports AccessLevel = lombok.AccessLevel
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports MultiDataSetIterator = org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator
Imports ND4JException = org.nd4j.linalg.exception.ND4JException
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


Namespace org.nd4j.autodiff.util

	''' <summary>
	''' Utilities for SameDiff training and inference
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor(access = AccessLevel.@PRIVATE) public class TrainingUtils
	Public Class TrainingUtils

		''' <summary>
		''' Stack batch outputs, like an output from <seealso cref="org.nd4j.autodiff.samediff.SameDiff.output(MultiDataSetIterator, String...)"/>
		''' </summary>
		Public Shared Function stackOutputs(ByVal outputs As IList(Of IDictionary(Of String, INDArray))) As IDictionary(Of String, INDArray)
			Dim outs As IDictionary(Of String, IList(Of INDArray)) = New Dictionary(Of String, IList(Of INDArray))()
			For Each batch As IDictionary(Of String, INDArray) In outputs
				For Each k As String In batch.Keys
					If Not outs.ContainsKey(k) Then
						outs(k) = New List(Of INDArray)()
					End If
					outs(k).Add(batch(k))
				Next k
			Next batch

			Dim ret As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
			For Each k As String In outs.Keys
				Try
					ret(k) = Nd4j.concat(0, CType(outs(k), List(Of INDArray)).ToArray())
				Catch e As Exception
					Throw New ND4JException("Error concatenating batch outputs", e)
				End Try
			Next k
			Return ret
		End Function

		''' <summary>
		''' Get a list of batch outputs for a single variable from a list of batch outputs for all variables
		''' </summary>
		Public Shared Function getSingleOutput(ByVal outputs As IList(Of IDictionary(Of String, INDArray)), ByVal output As String) As IList(Of INDArray)
			Dim batches As IList(Of INDArray) = New List(Of INDArray)()
			For Each batch As IDictionary(Of String, INDArray) In outputs
				batches.Add(batch(output))
			Next batch

			Return batches
		End Function
	End Class

End Namespace