Imports System
Imports System.Collections.Generic
Imports Lists = org.nd4j.shade.guava.collect.Lists
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports FileUtils = org.apache.commons.io.FileUtils
Imports IOFileFilter = org.apache.commons.io.filefilter.IOFileFilter
Imports RegexFileFilter = org.apache.commons.io.filefilter.RegexFileFilter
Imports AsyncDataSetIterator = org.nd4j.linalg.dataset.AsyncDataSetIterator
Imports FileSplitDataSetIterator = org.deeplearning4j.datasets.iterator.FileSplitDataSetIterator
Imports FileCallback = org.deeplearning4j.datasets.iterator.callbacks.FileCallback
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports InequalityHandling = org.nd4j.linalg.dataset.api.iterator.enums.InequalityHandling
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
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

Namespace org.deeplearning4j.datasets.iterator.parallel


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class FileSplitParallelDataSetIterator extends BaseParallelDataSetIterator
	<Serializable>
	Public Class FileSplitParallelDataSetIterator
		Inherits BaseParallelDataSetIterator

		Public Const DEFAULT_PATTERN As String = "dataset-%d.bin"
		Private pattern As String
		Private buffer As Integer

		Protected Friend asyncIterators As IList(Of DataSetIterator) = New List(Of DataSetIterator)()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public FileSplitParallelDataSetIterator(@NonNull File rootFolder, @NonNull String pattern, @NonNull FileCallback callback)
		Public Sub New(ByVal rootFolder As File, ByVal pattern As String, ByVal callback As FileCallback)
			Me.New(rootFolder, pattern, callback, Nd4j.AffinityManager.NumberOfDevices)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public FileSplitParallelDataSetIterator(@NonNull File rootFolder, @NonNull String pattern, @NonNull FileCallback callback, int numThreads)
		Public Sub New(ByVal rootFolder As File, ByVal pattern As String, ByVal callback As FileCallback, ByVal numThreads As Integer)
			Me.New(rootFolder, pattern, callback, numThreads, InequalityHandling.STOP_EVERYONE)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public FileSplitParallelDataSetIterator(@NonNull File rootFolder, @NonNull String pattern, @NonNull FileCallback callback, int numThreads, @NonNull InequalityHandling inequalityHandling)
		Public Sub New(ByVal rootFolder As File, ByVal pattern As String, ByVal callback As FileCallback, ByVal numThreads As Integer, ByVal inequalityHandling As InequalityHandling)
			Me.New(rootFolder, pattern, callback, numThreads, 2, inequalityHandling)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public FileSplitParallelDataSetIterator(@NonNull File rootFolder, @NonNull String pattern, @NonNull FileCallback callback, int numThreads, int bufferPerThread, @NonNull InequalityHandling inequalityHandling)
		Public Sub New(ByVal rootFolder As File, ByVal pattern As String, ByVal callback As FileCallback, ByVal numThreads As Integer, ByVal bufferPerThread As Integer, ByVal inequalityHandling As InequalityHandling)
			MyBase.New(numThreads)

			If Not rootFolder.exists() OrElse Not rootFolder.isDirectory() Then
				Throw New System.ArgumentException("Root folder should point to existing folder")
			End If

			Me.pattern = pattern
			Me.inequalityHandling = inequalityHandling
			Me.buffer = bufferPerThread

			Dim modifiedPattern As String = pattern.replaceAll("\%d", ".*.")

			Dim fileFilter As IOFileFilter = New RegexFileFilter(modifiedPattern)


			Dim files As IList(Of File) = New List(Of File)(FileUtils.listFiles(rootFolder, fileFilter, Nothing))
			log.debug("Files found: {}; Producers: {}", files.Count, numProducers)

			If files.Count = 0 Then
				Throw New System.ArgumentException("No suitable files were found")
			End If

			Dim numDevices As Integer = Nd4j.AffinityManager.NumberOfDevices
			Dim cnt As Integer = 0
			For Each part As IList(Of File) In Lists.partition(files, files.Count \ numThreads)
				' discard remainder
				If cnt >= numThreads Then
					Exit For
				End If

				Dim cDev As Integer = cnt Mod numDevices
				asyncIterators.Add(New AsyncDataSetIterator(New FileSplitDataSetIterator(part, callback), bufferPerThread, True, cDev))
				cnt += 1
			Next part

		End Sub

		Public Overrides Function hasNextFor(ByVal consumer As Integer) As Boolean
			If consumer >= numProducers OrElse consumer < 0 Then
				Throw New ND4JIllegalStateException("Non-existent consumer was requested")
			End If

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Return asyncIterators(consumer).hasNext()
		End Function

		Public Overrides Function nextFor(ByVal consumer As Integer) As DataSet
			If consumer >= numProducers OrElse consumer < 0 Then
				Throw New ND4JIllegalStateException("Non-existent consumer was requested")
			End If

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Return asyncIterators(consumer).next()
		End Function

		Protected Friend Overrides Sub reset(ByVal consumer As Integer)
			If consumer >= numProducers OrElse consumer < 0 Then
				Throw New ND4JIllegalStateException("Non-existent consumer was requested")
			End If

			asyncIterators(consumer).reset()
		End Sub



	End Class

End Namespace