Imports System.Collections.Generic
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Data = lombok.Data
Imports NonNull = lombok.NonNull

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

Namespace org.nd4j.imports.tensorflow


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Data public class TFImportStatus
	Public Class TFImportStatus

		''' <summary>
		''' The paths of the model(s) that have been investigated </summary>
		Private ReadOnly modelPaths As IList(Of String)
		''' <summary>
		''' The paths of the models that can't be imported, because they have 1 or more missing ops </summary>
		Private ReadOnly cantImportModelPaths As IList(Of String)
		''' <summary>
		''' The paths of the models that can't be read for some reason (corruption, etc?) </summary>
		Private ReadOnly readErrorModelPaths As IList(Of String)
		''' <summary>
		''' The total number of ops in all graphs </summary>
		Private ReadOnly totalNumOps As Integer
		''' <summary>
		''' The number of unique ops in all graphs </summary>
		Private ReadOnly numUniqueOps As Integer
		''' <summary>
		''' The (unique) names of all ops encountered in all graphs </summary>
		Private ReadOnly opNames As ISet(Of String)
		''' <summary>
		''' The number of times each operation was observed in all graphs </summary>
		Private ReadOnly opCounts As IDictionary(Of String, Integer)
		''' <summary>
		''' The (unique) names of all ops that were encountered, and can be imported, in all graphs </summary>
		Private ReadOnly importSupportedOpNames As ISet(Of String)
		''' <summary>
		''' The (unique) names of all ops that were encountered, and can NOT be imported (lacking import mapping) </summary>
		Private ReadOnly unsupportedOpNames As ISet(Of String)
		Private ReadOnly unsupportedOpModels As IDictionary(Of String, ISet(Of String))


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public TFImportStatus merge(@NonNull TFImportStatus other)
		Public Overridable Function merge(ByVal other As TFImportStatus) As TFImportStatus
			Dim newModelPaths As IList(Of String) = New List(Of String)(modelPaths)
			CType(newModelPaths, List(Of String)).AddRange(other.modelPaths)

			Dim newCantImportModelPaths As IList(Of String) = New List(Of String)(cantImportModelPaths)
			CType(newCantImportModelPaths, List(Of String)).AddRange(other.cantImportModelPaths)

			Dim newReadErrorModelPaths As IList(Of String) = New List(Of String)(readErrorModelPaths)
			CType(newReadErrorModelPaths, List(Of String)).AddRange(other.readErrorModelPaths)



			Dim newOpNames As ISet(Of String) = New HashSet(Of String)(opNames)
			newOpNames.addAll(other.opNames)

			Dim newOpCounts As IDictionary(Of String, Integer) = New Dictionary(Of String, Integer)(opCounts)
			For Each e As KeyValuePair(Of String, Integer) In other.opCounts.SetOfKeyValuePairs()
				newOpCounts(e.Key) = (If(newOpCounts.ContainsKey(e.Key), newOpCounts(e.Key), 0)) + e.Value
			Next e

			Dim newImportSupportedOpNames As ISet(Of String) = New HashSet(Of String)(importSupportedOpNames)
			newImportSupportedOpNames.addAll(other.importSupportedOpNames)

			Dim newUnsupportedOpNames As ISet(Of String) = New HashSet(Of String)(unsupportedOpNames)
			newUnsupportedOpNames.addAll(other.unsupportedOpNames)

			Dim countUnique As Integer = newImportSupportedOpNames.Count + newUnsupportedOpNames.Count

			Dim newUnsupportedOpModels As IDictionary(Of String, ISet(Of String)) = New Dictionary(Of String, ISet(Of String))()
			If unsupportedOpModels IsNot Nothing Then
				newUnsupportedOpModels.PutAll(unsupportedOpModels)
			End If
			If other.unsupportedOpModels IsNot Nothing Then
				For Each e As KeyValuePair(Of String, ISet(Of String)) In other.unsupportedOpModels.SetOfKeyValuePairs()
					If Not newUnsupportedOpModels.ContainsKey(e.Key) Then
						newUnsupportedOpModels(e.Key) = e.Value
					Else
						newUnsupportedOpModels(e.Key).addAll(e.Value)
					End If
				Next e
			End If


			Return New TFImportStatus(newModelPaths, newCantImportModelPaths, newReadErrorModelPaths, totalNumOps + other.totalNumOps, countUnique, newOpNames, newOpCounts, newImportSupportedOpNames, newUnsupportedOpNames, newUnsupportedOpModels)
		End Function

	End Class

End Namespace