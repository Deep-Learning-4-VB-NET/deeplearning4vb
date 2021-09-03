Imports System
Imports System.Threading
Imports JCommander = com.beust.jcommander.JCommander
Imports Parameter = com.beust.jcommander.Parameter
Imports ParameterException = com.beust.jcommander.ParameterException
Imports Data = lombok.Data
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports DL4JClassLoading = org.deeplearning4j.common.config.DL4JClassLoading
Imports StatsStorageRouter = org.deeplearning4j.core.storage.StatsStorageRouter
Imports RemoteUIStatsStorageRouter = org.deeplearning4j.core.storage.impl.RemoteUIStatsStorageRouter
Imports Model = org.deeplearning4j.nn.api.Model
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports ParallelWrapper = org.deeplearning4j.parallelism.ParallelWrapper
Imports ModelGuesser = org.deeplearning4j.core.util.ModelGuesser
Imports ModelSerializer = org.deeplearning4j.util.ModelSerializer
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

Namespace org.deeplearning4j.parallelism.main


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @Slf4j public class ParallelWrapperMain
	Public Class ParallelWrapperMain
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Parameter(names = {"--modelPath"}, description = "Path to the model", arity = 1, required = true) private String modelPath = null;
		Private modelPath As String = Nothing
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Parameter(names = {"--workers"}, description = "Number of workers", arity = 1) private int workers = 2;
		Private workers As Integer = 2
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Parameter(names = {"--prefetchSize"}, description = "The number of datasets to prefetch", arity = 1) private int prefetchSize = 16;
		Private prefetchSize As Integer = 16
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Parameter(names = {"--averagingFrequency"}, description = "The frequency for averaging parameters", arity = 1) private int averagingFrequency = 1;
		Private averagingFrequency As Integer = 1
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Parameter(names = {"--reportScore"}, description = "The subcommand to run", arity = 1) private boolean reportScore = false;
		Private reportScore As Boolean = False
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Parameter(names = {"--averageUpdaters"}, description = "Whether to average updaters", arity = 1) private boolean averageUpdaters = true;
		Private averageUpdaters As Boolean = True
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Parameter(names = {"--legacyAveraging"}, description = "Whether to use legacy averaging", arity = 1) private boolean legacyAveraging = true;
		Private legacyAveraging As Boolean = True
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Parameter(names = {"--dataSetIteratorFactoryClazz"}, description = "The fully qualified class name of the multi data set iterator class to use.", arity = 1) private String dataSetIteratorFactoryClazz = null;
		Private dataSetIteratorFactoryClazz As String = Nothing
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Parameter(names = {"--multiDataSetIteratorFactoryClazz"}, description = "The fully qualified class name of the multi data set iterator class to use.", arity = 1) private String multiDataSetIteratorFactoryClazz = null;
		Private multiDataSetIteratorFactoryClazz As String = Nothing
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Parameter(names = {"--modelOutputPath"}, description = "The fully qualified class name of the multi data set iterator class to use.", arity = 1, required = true) private String modelOutputPath = null;
		Private modelOutputPath As String = Nothing
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Parameter(names = {"--uiUrl"}, description = "The host:port of the ui to use (optional)", arity = 1) private String uiUrl = null;
		Private uiUrl As String = Nothing


		Private remoteUIRouter As RemoteUIStatsStorageRouter
		Private wrapper As ParallelWrapper

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void main(String[] args) throws Exception
		Public Shared Sub Main(ByVal args() As String)
			Call (New ParallelWrapperMain()).runMain(args)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void runMain(String... args) throws Exception
		Public Overridable Sub runMain(ParamArray ByVal args() As String)
			Dim jcmdr As New JCommander(Me)

			Try
				jcmdr.parse(args)
			Catch e As ParameterException
				Console.Error.WriteLine(e.Message)
				'User provides invalid input -> print the usage info
				jcmdr.usage()
				Try
					Thread.Sleep(500)
				Catch e2 As Exception
				End Try
				Environment.Exit(1)
			End Try

			run()

		End Sub


'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void run() throws Exception
		Public Overridable Sub run()

			Dim model As Model = ModelGuesser.loadModelGuess(modelPath)
			' ParallelWrapper will take care of load balancing between GPUs.
			wrapper = (New ParallelWrapper.Builder(model)).prefetchBuffer(prefetchSize).workers(workers).averagingFrequency(averagingFrequency).averageUpdaters(averageUpdaters).reportScoreAfterAveraging(reportScore).build()

			If dataSetIteratorFactoryClazz IsNot Nothing Then
				Dim dataSetIteratorProviderFactory As DataSetIteratorProviderFactory = DL4JClassLoading.createNewInstance(dataSetIteratorFactoryClazz)

				Dim dataSetIterator As DataSetIterator = dataSetIteratorProviderFactory.create()
				If uiUrl IsNot Nothing Then
					' it's important that the UI can report results from parallel training
					' there's potential for StatsListener to fail if certain properties aren't set in the model
					Dim remoteUIRouter As StatsStorageRouter = New RemoteUIStatsStorageRouter("http://" & uiUrl)
					Dim trainingListener As TrainingListener = DL4JClassLoading.createNewInstance("org.deeplearning4j.ui.model.stats.StatsListener", GetType(StatsStorageRouter), New Type() { GetType(StatsStorageRouter) }, New Object() { Nothing })
					wrapper.setListeners(remoteUIRouter, trainingListener)

				End If
				wrapper.fit(dataSetIterator)
				ModelSerializer.writeModel(model, New File(modelOutputPath), True)
			ElseIf multiDataSetIteratorFactoryClazz IsNot Nothing Then
				Dim multiDataSetProviderFactory As MultiDataSetProviderFactory = DL4JClassLoading.createNewInstance(multiDataSetIteratorFactoryClazz)

				Dim iterator As MultiDataSetIterator = multiDataSetProviderFactory.create()
				If uiUrl IsNot Nothing Then
					' it's important that the UI can report results from parallel training
					' there's potential for StatsListener to fail if certain properties aren't set in the model
					remoteUIRouter = New RemoteUIStatsStorageRouter("http://" & uiUrl)
					Dim trainingListener As TrainingListener = DL4JClassLoading.createNewInstance("org.deeplearning4j.ui.model.stats.StatsListener", GetType(TrainingListener), New Type(){ GetType(StatsStorageRouter) }, New Object(){ Nothing })
					wrapper.setListeners(remoteUIRouter, trainingListener)

				End If
				wrapper.fit(iterator)
				ModelSerializer.writeModel(model, New File(modelOutputPath), True)
			Else
				Throw New System.InvalidOperationException("Please provide a datasetiteraator or multi datasetiterator class")
			End If
		End Sub

		''' <summary>
		''' Stop the ParallelWrapper main. Mainly used for testing purposes
		''' </summary>
		Public Overridable Sub [stop]()
			If remoteUIRouter IsNot Nothing Then
				remoteUIRouter.shutdown()
			End If

			If wrapper IsNot Nothing Then
				Try
					wrapper.close()
				Catch t As Exception
					log.warn("ParallelWrapperMain.close(): Exception encountered trying to close ParallelWrapper instance", t)
					Throw New Exception(t)
				End Try
			End If
		End Sub
	End Class

End Namespace