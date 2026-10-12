Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Threading
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports FileUtils = org.apache.commons.io.FileUtils
Imports DataSetPreProcessor = org.nd4j.linalg.dataset.api.DataSetPreProcessor
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports ND4JFileUtils = org.nd4j.common.util.ND4JFileUtils

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
'ORIGINAL LINE: @Slf4j public class MiniBatchFileDataSetIterator implements org.nd4j.linalg.dataset.api.iterator.DataSetIterator
	<Serializable>
	Public Class MiniBatchFileDataSetIterator
		Implements DataSetIterator

		Private batchSize As Integer
		Private paths As IList(Of String())
		Private currIdx As Integer
'JAVA TO VB CONVERTER NOTE: The field rootDir was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private rootDir_Conflict As File
		Private totalExamples As Integer
		Private totalLabels As Integer
		Private totalBatches As Integer = -1
		Private dataSetPreProcessor As DataSetPreProcessor



		''' 
		''' <param name="baseData"> the base dataset </param>
		''' <param name="batchSize"> the batch size to split by </param>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public MiniBatchFileDataSetIterator(DataSet baseData, int batchSize) throws IOException
		Public Sub New(ByVal baseData As DataSet, ByVal batchSize As Integer)
			Me.New(baseData, batchSize, True)

		End Sub

		''' 
		''' <param name="baseData"> the base dataset </param>
		''' <param name="batchSize"> the batch size to split by </param>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public MiniBatchFileDataSetIterator(DataSet baseData, int batchSize, boolean delete, File rootDir) throws IOException
		Public Sub New(ByVal baseData As DataSet, ByVal batchSize As Integer, ByVal delete As Boolean, ByVal rootDir As File)
			If baseData.numExamples() < batchSize Then
				Throw New IllegalAccessError("Number of examples smaller than batch size")
			End If
			Me.batchSize = batchSize
			Me.rootDir_Conflict = New File(rootDir, System.Guid.randomUUID().ToString())
			Me.rootDir_Conflict.mkdirs()
			If delete Then
				Runtime.getRuntime().addShutdownHook(New Thread(Sub()
				Try
					FileUtils.deleteDirectory(MiniBatchFileDataSetIterator.this.rootDir)
				Catch e As IOException
					log.error("",e)
				End Try
				End Sub}))
			currIdx = 0
			paths = New List(Of String())()
			totalExamples = baseData.numExamples()
			totalLabels = baseData.numOutcomes()
			Dim offset As Integer = 0
			totalBatches = baseData.numExamples() \ batchSize
			Dim i As Integer = 0
			Do While i < baseData.numExamples() \ batchSize
				paths.Add(writeData(New DataSet(baseData.Features.get(NDArrayIndex.interval(offset, offset + batchSize)), baseData.Labels.get(NDArrayIndex.interval(offset, offset + batchSize)))))
				offset += batchSize
				If offset >= totalExamples Then
					Exit Do
				End If
				i += 1
			Loop
			End If

		''' 
		''' <param name="baseData"> the base dataset </param>
		''' <param name="batchSize"> the batch size to split by </param>
		''' <exception cref="IOException"> </exception>
		public MiniBatchFileDataSetIterator(DataSet baseData, Integer batchSize, Boolean delete) throws IOException
		If True Then
			Me.New(baseData, batchSize, delete, ND4JFileUtils.TempDir)
		End If

		public DataSet [next](Integer num)
		If True Then
			Throw New System.NotSupportedException("Unable to load custom number of examples")
		End If

		public Integer inputColumns()
		If True Then
			Return 0
		End If

		public Integer totalOutcomes()
		If True Then
			Return totalLabels
		End If

		public Boolean resetSupported()
		If True Then
			Return True
		End If

		public Boolean asyncSupported()
		If True Then
			Return True
		End If

		public void reset()
		If True Then
			currIdx = 0
		End If

		public Integer batch()
		If True Then
			Return batchSize
		End If

		public void setPreProcessor(DataSetPreProcessor preProcessor)
		If True Then
			Me.dataSetPreProcessor = preProcessor
		End If

		public DataSetPreProcessor PreProcessor
		If True Then
			Return dataSetPreProcessor
		End If

		public IList(Of String) getLabels()
		If True Then
			Return Nothing
		End If

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
		public Boolean hasNext()
		If True Then
			Return currIdx < totalBatches
		End If

		public void remove()
		If True Then
			'no opt;
		End If

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
		public DataSet [next]()
		If True Then
			Try
				Dim ret As DataSet = read(currIdx)
				If dataSetPreProcessor IsNot Nothing Then
					dataSetPreProcessor.preProcess(ret)
				End If
				currIdx += 1

				Return ret
			Catch e As IOException
				Throw New System.InvalidOperationException("Unable to read dataset")
			End Try
		End If

		private DataSet read(Integer idx) throws IOException
		If True Then
			Dim bis As New BufferedInputStream(New FileStream(paths(idx)(0), FileMode.Open, FileAccess.Read))
			Dim dis As New DataInputStream(bis)
			Dim labelInputStream As New BufferedInputStream(New FileStream(paths(idx)(1), FileMode.Open, FileAccess.Read))
			Dim labelDis As New DataInputStream(labelInputStream)
			Dim d As New DataSet(Nd4j.read(dis), Nd4j.read(labelDis))
			dis.close()
			labelDis.close()
			Return d
		End If


		private String() writeData(DataSet write) throws IOException
		If True Then
			Dim ret(1) As String
			Dim dataSetId As String = System.Guid.randomUUID().ToString()
			Dim dataOut As New BufferedOutputStream(New FileStream(rootDir, dataSetId & ".bin", FileMode.Create, FileAccess.Write))
			Dim dos As New DataOutputStream(dataOut)
			Nd4j.write(write.getFeatures(), dos)
			dos.flush()
			dos.close()


			Dim dataOutLabels As New BufferedOutputStream(New FileStream(rootDir, dataSetId & ".labels.bin", FileMode.Create, FileAccess.Write))
			Dim dosLabels As New DataOutputStream(dataOutLabels)
			Nd4j.write(write.getLabels(), dosLabels)
			dosLabels.flush()
			dosLabels.close()
			ret(0) = (New File(rootDir, dataSetId & ".bin")).getAbsolutePath()
			ret(1) = (New File(rootDir, dataSetId & ".labels.bin")).getAbsolutePath()
			Return ret

		End If

		public File Me.RootDir
		If True Then
			Return rootDir
		End If

		public void setRootDir(File rootDir)
		If True Then
			Me.rootDir_Conflict = rootDir
		End If
		End Sub

End Namespace