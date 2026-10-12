Imports System
Imports System.IO
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports FileUtils = org.apache.commons.io.FileUtils
Imports IOUtils = org.apache.commons.io.IOUtils
Imports DL4JFileUtils = org.deeplearning4j.common.util.DL4JFileUtils
Imports DL4JSystemProperties = org.deeplearning4j.common.config.DL4JSystemProperties
Imports Model = org.deeplearning4j.nn.api.Model
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports KerasModelImport = org.deeplearning4j.nn.modelimport.keras.KerasModelImport
Imports ModelSerializer = org.deeplearning4j.util.ModelSerializer
Imports org.nd4j.linalg.dataset.api.preprocessor

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

Namespace org.deeplearning4j.core.util


	''' <summary>
	''' Guess a model from the given path
	''' @author Adam Gibson
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class ModelGuesser
	Public Class ModelGuesser


		''' <summary>
		''' A facade for <seealso cref="ModelSerializer.restoreNormalizerFromInputStream(InputStream)"/> </summary>
		''' <param name="is"> the input stream to load form </param>
		''' <returns> the loaded normalizer </returns>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static org.nd4j.linalg.dataset.api.preprocessor.Normalizer<?> loadNormalizer(InputStream is) throws IOException
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
		Public Shared Function loadNormalizer(ByVal [is] As Stream) As Normalizer(Of Object)
			Return ModelSerializer.restoreNormalizerFromInputStream([is])
		End Function

		''' <summary>
		''' A facade for <seealso cref="ModelSerializer.restoreNormalizerFromFile(File)"/> </summary>
		''' <param name="path"> the path to the file </param>
		''' <returns> the loaded normalizer </returns>
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: public static org.nd4j.linalg.dataset.api.preprocessor.Normalizer<?> loadNormalizer(String path)
		Public Shared Function loadNormalizer(ByVal path As String) As Normalizer(Of Object)
			Try
				Return ModelSerializer.restoreNormalizerFromFile(New File(path))
			Catch e As IOException
				Throw New Exception(e)
			End Try
		End Function



		''' <summary>
		''' Load the model from the given file path </summary>
		''' <param name="path"> the path of the file to "guess"
		''' </param>
		''' <returns> the loaded model </returns>
		''' <exception cref="Exception"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static Object loadConfigGuess(String path) throws Exception
		Public Shared Function loadConfigGuess(ByVal path As String) As Object
			Dim input As String = FileUtils.readFileToString(New File(path))
			'note here that we load json BEFORE YAML. YAML
			'turns out to load just fine *accidentally*
			Try
				Return MultiLayerConfiguration.fromJson(input)
			Catch e As Exception
				log.warn("Tried multi layer config from json", e)
				Try
					Return KerasModelImport.importKerasModelConfiguration(path)
				Catch e1 As Exception
					log.warn("Tried keras model config", e)
					Try
						Return KerasModelImport.importKerasSequentialConfiguration(path)
					Catch e2 As Exception
						log.warn("Tried keras sequence config", e)
						Try
							Return ComputationGraphConfiguration.fromJson(input)
						Catch e3 As Exception
							log.warn("Tried computation graph from json")
							Try
								Return MultiLayerConfiguration.fromYaml(input)
							Catch e4 As Exception
								log.warn("Tried multi layer configuration from yaml")
								Try
									Return ComputationGraphConfiguration.fromYaml(input)
								Catch e5 As Exception
									Throw New ModelGuesserException("Unable to load configuration from path " & path & " (invalid config file or not a known config type)")
								End Try
							End Try
						End Try
					End Try
				End Try
			End Try
		End Function


		''' <summary>
		''' Load the model from the given input stream </summary>
		''' <param name="stream"> the path of the file to "guess"
		''' </param>
		''' <returns> the loaded model </returns>
		''' <exception cref="Exception"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static Object loadConfigGuess(InputStream stream) throws Exception
		Public Shared Function loadConfigGuess(ByVal stream As Stream) As Object
			Dim p As String = System.getProperty(DL4JSystemProperties.DL4J_TEMP_DIR_PROPERTY)
			Dim tmp As File = DL4JFileUtils.createTempFile("model-" & System.Guid.randomUUID().ToString(), "bin")
			Dim bufferedOutputStream As New BufferedOutputStream(New FileStream(tmp, FileMode.Create, FileAccess.Write))
			IOUtils.copy(stream, bufferedOutputStream)
			bufferedOutputStream.flush()
			bufferedOutputStream.close()
			tmp.deleteOnExit()
			Try
				Return loadConfigGuess(tmp.getAbsolutePath())
			Finally
				tmp.delete()
			End Try
		End Function

		''' <summary>
		''' Load the model from the given file path </summary>
		''' <param name="path"> the path of the file to "guess"
		''' </param>
		''' <returns> the loaded model </returns>
		''' <exception cref="Exception"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static org.deeplearning4j.nn.api.Model loadModelGuess(String path) throws Exception
		Public Shared Function loadModelGuess(ByVal path As String) As Model
			Try
				Return ModelSerializer.restoreMultiLayerNetwork(New File(path), True)
			Catch e As Exception
				log.warn("Tried multi layer network")
				Try
					Return ModelSerializer.restoreComputationGraph(New File(path), True)
				Catch e1 As Exception
					log.warn("Tried computation graph")
					Try
						Return ModelSerializer.restoreMultiLayerNetwork(New File(path), False)
					Catch e4 As Exception
						Try
							Return ModelSerializer.restoreComputationGraph(New File(path), False)
						Catch e5 As Exception
							Try
								Return KerasModelImport.importKerasModelAndWeights(path)
							Catch e2 As Exception
								log.warn("Tried multi layer network keras")
								Try
									Return KerasModelImport.importKerasSequentialModelAndWeights(path)

								Catch e3 As Exception
									Throw New ModelGuesserException("Unable to load model from path " & path & " (invalid model file or not a known model type)")
								End Try
							End Try
						End Try
					End Try
				End Try
			End Try
		End Function


		''' <summary>
		''' Load the model from the given input stream. The content of the stream is written to a temporary location,
		''' see <seealso cref="DL4JSystemProperties.DL4J_TEMP_DIR_PROPERTY"/>
		''' </summary>
		''' <param name="stream"> the path of the file to "guess"
		''' </param>
		''' <returns> the loaded model </returns>
		''' <exception cref="Exception"> </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static org.deeplearning4j.nn.api.Model loadModelGuess(InputStream stream) throws Exception
		Public Shared Function loadModelGuess(ByVal stream As Stream) As Model
			Return loadModelGuess(stream, Nothing)
		End Function

		''' <summary>
		''' As per <seealso cref="loadModelGuess(InputStream)"/> but (optionally) allows copying to the specified temporary directory </summary>
		''' <param name="stream">        Stream of the model file </param>
		''' <param name="tempDirectory"> Temporary/working directory. May be null. </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static org.deeplearning4j.nn.api.Model loadModelGuess(InputStream stream, File tempDirectory) throws Exception
		Public Shared Function loadModelGuess(ByVal stream As Stream, ByVal tempDirectory As File) As Model
			'Currently (Nov 2017): KerasModelImport doesn't support loading from input streams
			'Simplest solution here: write to a temporary file
			Dim f As File
			If tempDirectory Is Nothing Then
				f = DL4JFileUtils.createTempFile("loadModelGuess",".bin")
			Else
				f = File.createTempFile("loadModelGuess", ".bin", tempDirectory)
			End If
			f.deleteOnExit()


			Try
					Using os As Stream = New BufferedOutputStream(New FileStream(f, FileMode.Create, FileAccess.Write))
					IOUtils.copy(stream, os)
					os.Flush()
					Return loadModelGuess(f.getAbsolutePath())
					End Using
			Catch e As ModelGuesserException
				Throw New ModelGuesserException("Unable to load model from input stream (invalid model file not a known model type)")
			Finally
				f.delete()
			End Try
		End Function



	End Class

End Namespace