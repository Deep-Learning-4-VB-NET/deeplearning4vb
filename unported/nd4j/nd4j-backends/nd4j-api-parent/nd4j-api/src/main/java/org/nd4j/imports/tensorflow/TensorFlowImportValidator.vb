Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports ArchiveEntry = org.apache.commons.compress.archivers.ArchiveEntry
Imports TarArchiveInputStream = org.apache.commons.compress.archivers.tar.TarArchiveInputStream
Imports BZip2CompressorInputStream = org.apache.commons.compress.compressors.bzip2.BZip2CompressorInputStream
Imports FileUtils = org.apache.commons.io.FileUtils
Imports FilenameUtils = org.apache.commons.io.FilenameUtils
Imports CloseShieldInputStream = org.apache.commons.io.input.CloseShieldInputStream
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DifferentialFunctionClassHolder = org.nd4j.imports.converters.DifferentialFunctionClassHolder
Imports TFGraphMapper = org.nd4j.imports.graphmapper.tf.TFGraphMapper
Imports ArchiveUtils = org.nd4j.common.util.ArchiveUtils
Imports GraphDef = org.tensorflow.framework.GraphDef
Imports NodeDef = org.tensorflow.framework.NodeDef

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
'ORIGINAL LINE: @Slf4j public class TensorFlowImportValidator
	Public Class TensorFlowImportValidator

		''' <summary>
		''' Recursively scan the specified directory for .pb files, and evaluate which operations/graphs can/can't be imported </summary>
		''' <param name="directory"> Directory to scan </param>
		''' <returns> Status for TensorFlow import for all models in </returns>
		''' <exception cref="IOException"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static TFImportStatus checkAllModelsForImport(@NonNull File directory) throws IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Shared Function checkAllModelsForImport(ByVal directory As File) As TFImportStatus
			Return checkModelForImport(directory, False)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static TFImportStatus checkAllModelsForImport(@NonNull File directory, boolean includeArchives) throws IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Shared Function checkAllModelsForImport(ByVal directory As File, ByVal includeArchives As Boolean) As TFImportStatus

			Dim fileExts As IList(Of String) = New List(Of String)()
			fileExts.Add("pb")
			If includeArchives Then
				CType(fileExts, List(Of String)).AddRange(New List(Of String) From {"zip", "tar.gz", "gzip", "tgz", "gz", "7z", "tar.bz2", "tar.gz2", "tar.lz", "tar.lzma", "tg", "tar"})
			End If

			Return checkAllModelsForImport(directory, CType(fileExts, List(Of String)).ToArray())
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static TFImportStatus checkAllModelsForImport(File directory, String[] fileExtensions) throws IOException
		Public Shared Function checkAllModelsForImport(ByVal directory As File, ByVal fileExtensions() As String) As TFImportStatus
			Preconditions.checkState(directory.isDirectory(), "Specified directory %s is not actually a directory", directory)


			Dim files As ICollection(Of File) = FileUtils.listFiles(directory, fileExtensions, True)
			Preconditions.checkState(files.Count > 0, "No model files found in directory %s", directory)

			Dim status As TFImportStatus = Nothing
			For Each f As File In files
				If isArchiveFile(f) Then
					Dim p As String = f.getAbsolutePath()
					log.info("Checking archive file for .pb files: " & p)

					Dim ext As String = FilenameUtils.getExtension(p).ToLower()
					Select Case ext
						Case "zip"
							Dim filesInZip As IList(Of String)
							Try
								filesInZip = ArchiveUtils.zipListFiles(f)
							Catch t As Exception
								log.warn("Unable to read from file, skipping: {}", f.getAbsolutePath(), t)
								Continue For
							End Try
							For Each s As String In filesInZip
								If s.EndsWith(".pb", StringComparison.Ordinal) Then
									Using zf As New java.util.zip.ZipFile(f), [is] As Stream = zf.getInputStream(zf.getEntry(s))
										Dim p2 As String = p & "/" & s
										log.info("Found possible frozen model (.pb) file in zip archive: {}", p2)
										Dim currStatus As TFImportStatus = checkModelForImport(p2, [is], False)
										If currStatus.getCantImportModelPaths() IsNot Nothing AndAlso Not currStatus.getCantImportModelPaths().isEmpty() Then
											log.info("Unable to load - not a frozen model .pb file: {}", p2)
										Else
											log.info("Found frozen model .pb file in archive: {}", p2)
										End If
										status = (If(status Is Nothing, currStatus, status.merge(currStatus)))
									End Using
								End If
							Next s
						Case "tar", "tar.gz", "tar.bz2", "tgz", "gz", "bz2"
							If p.EndsWith(".tar.gz", StringComparison.Ordinal) OrElse p.EndsWith(".tgz", StringComparison.Ordinal) OrElse p.EndsWith(".tar", StringComparison.Ordinal) OrElse p.EndsWith(".tar.bz2", StringComparison.Ordinal) Then
								Dim isTar As Boolean = p.EndsWith(".tar", StringComparison.Ordinal)
								Dim filesInTarGz As IList(Of String)
								Try
									filesInTarGz = If(isTar, ArchiveUtils.tarListFiles(f), ArchiveUtils.tarGzListFiles(f))
								Catch t As Exception
									log.warn("Unable to read from file, skipping: {}", f.getAbsolutePath(), t)
									Continue For
								End Try
								For Each s As String In filesInTarGz
									If s.EndsWith(".pb", StringComparison.Ordinal) Then
										Dim [is] As TarArchiveInputStream
										If p.EndsWith(".tar", StringComparison.Ordinal) Then
											[is] = New TarArchiveInputStream(New BufferedInputStream(New FileStream(f, FileMode.Open, FileAccess.Read)))
										ElseIf p.EndsWith(".tar.gz", StringComparison.Ordinal) OrElse p.EndsWith(".tgz", StringComparison.Ordinal) Then
											[is] = New TarArchiveInputStream(New GZIPInputStream(New BufferedInputStream(New FileStream(f, FileMode.Open, FileAccess.Read))))
										ElseIf p.EndsWith(".tar.bz2", StringComparison.Ordinal) Then
											[is] = New TarArchiveInputStream(New BZip2CompressorInputStream(New BufferedInputStream(New FileStream(f, FileMode.Open, FileAccess.Read))))
										Else
											Throw New Exception("Can't parse file type: " & s)
										End If

										Try
											Dim p2 As String = p & "/" & s
											log.info("Found possible frozen model (.pb) file in {} archive: {}", ext, p2)

											Dim entry As ArchiveEntry
											Dim found As Boolean = False
											entry = [is].getNextTarEntry()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while((entry = is.getNextTarEntry()) != null)
											Do While entry IsNot Nothing
												Dim name As String = entry.getName()
												If s.Equals(name) Then
													'Found entry we want...
													Dim currStatus As TFImportStatus = checkModelForImport(p2, New CloseShieldInputStream([is]), False)
													If currStatus.getCantImportModelPaths() IsNot Nothing AndAlso Not currStatus.getCantImportModelPaths().isEmpty() Then
														log.info("Unable to load - not a frozen model .pb file: {}", p2)
													Else
														log.info("Found frozen model .pb file in archive: {}", p2)
													End If
													status = (If(status Is Nothing, currStatus, status.merge(currStatus)))
													found = True
												End If
													entry = [is].getNextTarEntry()
											Loop
											Preconditions.checkState(found, "Could not find expected tar entry in file: " & p2)
										Finally
											[is].Close()
										End Try
									End If
								Next s
								Exit Select
							End If
							'Fall through for .gz - FilenameUtils.getExtension("x.tar.gz") returns "gz" :/
						Case "gzip"
							'Assume single file...
							Using [is] As Stream = New java.util.zip.GZIPInputStream(New BufferedInputStream(New FileStream(f, FileMode.Open, FileAccess.Read)))
								Try
									Dim currStatus As TFImportStatus = checkModelForImport(f.getAbsolutePath(), [is], False)
									status = (If(status Is Nothing, currStatus, status.merge(currStatus)))
								Catch t As Exception
									log.warn("Unable to read from file, skipping: {}", f.getAbsolutePath(), t)
									Continue For
								End Try
							End Using
						Case Else
							Throw New System.NotSupportedException("Archive type not yet implemented: " & f.getAbsolutePath())
					End Select
				Else
					log.info("Checking model file: " & f.getAbsolutePath())
					Dim currStatus As TFImportStatus = checkModelForImport(f)
					status = (If(status Is Nothing, currStatus, status.merge(currStatus)))
				End If

				Console.WriteLine("DONE FILE: " & f.getAbsolutePath() & " - totalOps = " & (If(status Is Nothing, 0, status.getOpNames().size())) & " - supported ops: " & (If(status Is Nothing, 0, status.getImportSupportedOpNames().size())) & " - unsupported ops: " & (If(status Is Nothing, 0, status.getUnsupportedOpNames().size())))
			Next f
			Return status
		End Function

		Public Shared Function isArchiveFile(ByVal f As File) As Boolean
			Return Not f.getPath().EndsWith(".pb")
		End Function

		''' <summary>
		''' See <seealso cref="checkModelForImport(File)"/>. Defaults to exceptionOnRead = false
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static TFImportStatus checkModelForImport(@NonNull File file) throws IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Shared Function checkModelForImport(ByVal file As File) As TFImportStatus
			Return checkModelForImport(file, False)
		End Function

		''' <summary>
		''' Check whether the TensorFlow frozen model (protobuf format) can be imported into SameDiff or not </summary>
		''' <param name="file">            Protobuf file </param>
		''' <param name="exceptionOnRead"> If true, and the file can't be read, throw an exception. If false, return an "empty" TFImportStatus </param>
		''' <returns> Status for importing the file </returns>
		''' <exception cref="IOException"> If error </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static TFImportStatus checkModelForImport(@NonNull File file, boolean exceptionOnRead) throws IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Shared Function checkModelForImport(ByVal file As File, ByVal exceptionOnRead As Boolean) As TFImportStatus
			Using [is] As Stream = New FileStream(file, FileMode.Open, FileAccess.Read)
				Return checkModelForImport(file.getAbsolutePath(), [is], exceptionOnRead)
			End Using
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static TFImportStatus checkModelForImport(String path, InputStream is, boolean exceptionOnRead) throws IOException
		Public Shared Function checkModelForImport(ByVal path As String, ByVal [is] As Stream, ByVal exceptionOnRead As Boolean) As TFImportStatus

			Try
				Dim opCount As Integer = 0
				Dim opNames As ISet(Of String) = New HashSet(Of String)()
				Dim opCounts As IDictionary(Of String, Integer) = New Dictionary(Of String, Integer)()

				Using bis As Stream = New BufferedInputStream([is])
					Dim graphDef As GraphDef = GraphDef.parseFrom(bis)
					Dim nodes As IList(Of NodeDef) = New List(Of NodeDef)(graphDef.getNodeCount())
					Dim i As Integer=0
					Do While i<graphDef.getNodeCount()
						nodes.Add(graphDef.getNode(i))
						i += 1
					Loop

					If nodes.Count = 0 Then
						Throw New System.InvalidOperationException("Error loading model for import - loaded graph def has no nodes (empty/corrupt file?): " & path)
					End If

					For Each nd As NodeDef In nodes
						If TFGraphMapper.isVariableNode(nd) OrElse TFGraphMapper.isPlaceHolder(nd) Then
							Continue For
						End If

						Dim op As String = nd.getOp()
						opNames.Add(op)
						Dim soFar As Integer = If(opCounts.ContainsKey(op), opCounts(op), 0)
						opCounts(op) = soFar + 1
						opCount += 1
					Next nd
				End Using

				Dim importSupportedOpNames As ISet(Of String) = New HashSet(Of String)()
				Dim unsupportedOpNames As ISet(Of String) = New HashSet(Of String)()
				Dim unsupportedOpModel As IDictionary(Of String, ISet(Of String)) = New Dictionary(Of String, ISet(Of String))()

				For Each s As String In opNames
					If DifferentialFunctionClassHolder.Instance.getOpWithTensorflowName(s) IsNot Nothing Then
						importSupportedOpNames.Add(s)
					Else
						unsupportedOpNames.Add(s)
						If unsupportedOpModel.ContainsKey(s) Then
							Continue For
						Else
							Dim l As ISet(Of String) = New HashSet(Of String)()
							l.Add(path)
							unsupportedOpModel(s) = l
						End If

					End If
				Next s




				Return New TFImportStatus(Collections.singletonList(path),If(unsupportedOpNames.Count > 0, Collections.singletonList(path), Enumerable.Empty(Of String)()), Enumerable.Empty(Of String)(), opCount, opNames.Count, opNames, opCounts, importSupportedOpNames, unsupportedOpNames, unsupportedOpModel)
			Catch t As Exception
				If exceptionOnRead Then
					Throw New IOException("Error reading model from path " & path & " - not a TensorFlow frozen model in ProtoBuf format?", t)
				End If
				log.warn("Failed to import model from: " & path & " - not a TensorFlow frozen model in ProtoBuf format?", t)
				Return New TFImportStatus(Enumerable.Empty(Of String)(), Enumerable.Empty(Of String)(), Collections.singletonList(path), 0, 0, Enumerable.Empty(Of String)(), Enumerable.Empty(Of String, Integer)(), Enumerable.Empty(Of String)(), Enumerable.Empty(Of String)(), Enumerable.Empty(Of String, ISet(Of String))())
			End Try
		End Function
	End Class

End Namespace