Imports System.Collections.Generic
Imports Lists = org.nd4j.shade.guava.collect.Lists
Imports Maps = org.nd4j.shade.guava.collect.Maps
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Builder = lombok.Builder
Imports Data = lombok.Data
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports DataNormalization = org.nd4j.linalg.dataset.api.preprocessor.DataNormalization

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

Namespace org.nd4j.linalg.dataset


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Builder @Data public class BalanceMinibatches
	Public Class BalanceMinibatches
		Private dataSetIterator As DataSetIterator
		Private numLabels As Integer
		Private paths As IDictionary(Of Integer, IList(Of File)) = Maps.newHashMap()
		Private miniBatchSize As Integer = -1
		Private rootDir As New File("minibatches")
		Private rootSaveDir As New File("minibatchessave")
		Private labelRootDirs As IList(Of File) = New List(Of File)()
		Private dataNormalization As DataNormalization

		''' <summary>
		''' Generate a balanced
		''' dataset minibatch fileset.
		''' </summary>
		Public Overridable Sub balance()
			If Not rootDir.exists() Then
				rootDir.mkdirs()
			End If
			If Not rootSaveDir.exists() Then
				rootSaveDir.mkdirs()
			End If

			If paths Is Nothing Then
				paths = Maps.newHashMap()
			End If
			If labelRootDirs Is Nothing Then
				labelRootDirs = Lists.newArrayList()
			End If

			For i As Integer = 0 To numLabels - 1
				paths(i) = New List(Of File)()
				labelRootDirs.Add(New File(rootDir, i.ToString()))
			Next i


			'lay out each example in their respective label directories tracking the paths along the way
			Do While dataSetIterator.MoveNext()
				Dim [next] As DataSet = dataSetIterator.Current
				'infer minibatch size from iterator
				If miniBatchSize < 0 Then
					miniBatchSize = [next].numExamples()
				End If
				Dim i As Integer = 0
				Do While i < [next].numExamples()
					Dim currExample As DataSet = [next].get(i)
					If Not labelRootDirs(currExample.outcome()).exists() Then
						labelRootDirs(currExample.outcome()).mkdirs()
					End If

					'individual example will be saved to: labelrootdir/examples.size()
					Dim example As New File(labelRootDirs(currExample.outcome()), (paths(currExample.outcome()).Count).ToString())
					currExample.save(example)
					paths(currExample.outcome()).Add(example)
					i += 1
				Loop
			Loop

			Dim numsSaved As Integer = 0
			'loop till all file paths have been removed
			Do While paths.Count > 0
				Dim miniBatch As IList(Of DataSet) = New List(Of DataSet)()
				Do While miniBatch.Count < miniBatchSize AndAlso paths.Count > 0
					For i As Integer = 0 To numLabels - 1
						If paths(i) IsNot Nothing AndAlso paths(i).Count > 0 Then
							Dim d As New DataSet()
							d.load(paths(i).RemoveAt(0))
							miniBatch.Add(d)
						Else
							paths.Remove(i)
						End If
					Next i
				Loop

				If Not rootSaveDir.exists() Then
					rootSaveDir.mkdirs()
				End If
				'save with an incremental count of the number of minibatches saved
				If miniBatch.Count > 0 Then
					Dim merge As DataSet = DataSet.merge(miniBatch)
					If dataNormalization IsNot Nothing Then
						dataNormalization.transform(merge)
					End If
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: merge.save(new java.io.File(rootSaveDir, String.format("dataset-%d.bin", numsSaved++)));
					merge.save(New File(rootSaveDir, String.Format("dataset-{0:D}.bin", numsSaved)))
						numsSaved += 1
				End If


			Loop

		End Sub

	End Class

End Namespace