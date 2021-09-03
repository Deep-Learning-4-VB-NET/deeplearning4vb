﻿Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Microsoft.VisualBasic
Imports Files = org.nd4j.shade.guava.io.Files
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports IOUtils = org.apache.commons.io.IOUtils
Imports Model = org.deeplearning4j.nn.api.Model
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports BaseTrainingListener = org.deeplearning4j.optimize.api.BaseTrainingListener
Imports ModelSerializer = org.deeplearning4j.util.ModelSerializer
Imports Preconditions = org.nd4j.common.base.Preconditions

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

Namespace org.deeplearning4j.optimize.listeners


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class CheckpointListener extends org.deeplearning4j.optimize.api.BaseTrainingListener implements Serializable
	<Serializable>
	Public Class CheckpointListener
		Inherits BaseTrainingListener

		Private Enum KeepMode
			ALL
			LAST
			LAST_AND_EVERY
		End Enum
		Private Shared ReadOnly MODEL_TYPES() As String = {"MultiLayerNetwork", "ComputationGraph", "Model"}

		Private rootDir As File
		Private keepMode As KeepMode
		Private keepLast As Integer
		Private keepEvery As Integer
		Private logSaving As Boolean
		Private deleteExisting As Boolean

		Private saveEveryNEpochs As Integer?
		Private saveEveryNIterations As Integer?
		Private saveEveryNIterSinceLast As Boolean
		Private saveEveryAmount As Long?
		Private saveEveryUnit As TimeUnit
		Private saveEveryMs As Long?
		Private saveEverySinceLast As Boolean

		Private lastCheckpointNum As Integer = -1
		Private checkpointRecordFile As File

'JAVA TO VB CONVERTER NOTE: The field lastCheckpoint was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private lastCheckpoint_Conflict As Checkpoint
		Private startTime As Long = -1
		Private startIter As Integer = -1
		Private lastSaveEveryMsNoSinceLast As Long?

		Private Sub New(ByVal builder As Builder)
			Me.rootDir = builder.rootDir
			Me.keepMode = builder.keepMode
			Me.keepLast = builder.keepLast_Conflict
			Me.keepEvery = builder.keepEvery
			Me.logSaving = builder.logSaving_Conflict
			Me.deleteExisting = builder.deleteExisting_Conflict

			Me.saveEveryNEpochs = builder.saveEveryNEpochs_Conflict
			Me.saveEveryNIterations = builder.saveEveryNIterations_Conflict
			Me.saveEveryNIterSinceLast = builder.saveEveryNIterSinceLast
			Me.saveEveryAmount = builder.saveEveryAmount
			Me.saveEveryUnit = builder.saveEveryUnit
			Me.saveEverySinceLast = builder.saveEverySinceLast

			If saveEveryAmount IsNot Nothing Then
				saveEveryMs = TimeUnit.MILLISECONDS.convert(saveEveryAmount, saveEveryUnit)
			End If

			Me.checkpointRecordFile = New File(rootDir, "checkpointInfo.txt")
			If Me.checkpointRecordFile.exists() AndAlso Me.checkpointRecordFile.length() > 0 Then

				If deleteExisting Then
					'Delete any files matching:
					'"checkpoint_" + checkpointNum + "_" + modelType + ".zip";
					Me.checkpointRecordFile.delete()
					Dim files() As File = rootDir.listFiles()
					If files IsNot Nothing AndAlso files.Length > 0 Then
						For Each f As File In files
							Dim name As String = f.getName()
							If name.StartsWith("checkpoint_", StringComparison.Ordinal) AndAlso (name.EndsWith("MultiLayerNetwork.zip", StringComparison.Ordinal) OrElse name.EndsWith("ComputationGraph.zip", StringComparison.Ordinal)) Then
								f.delete()
							End If
						Next f
					End If
				Else
					Throw New System.InvalidOperationException("Detected existing checkpoint files at directory " & rootDir.getAbsolutePath() & ". Use deleteExisting(true) to delete existing checkpoint files when present.")
				End If
			End If
		End Sub

		Public Overrides Sub onEpochEnd(ByVal model As Model)
			Dim epochsDone As Integer = getEpoch(model) + 1
			If saveEveryNEpochs IsNot Nothing AndAlso epochsDone > 0 AndAlso epochsDone Mod saveEveryNEpochs = 0 Then
				'Save:
				saveCheckpoint(model)
			End If
			'General saving conditions: don't need to check here - will check in iterationDone
		End Sub

		Public Overrides Sub iterationDone(ByVal model As Model, ByVal iteration As Integer, ByVal epoch As Integer)
			If startTime < 0 Then
				startTime = DateTimeHelper.CurrentUnixTimeMillis()
				startIter = iteration
				Return
			End If

			'Check iterations saving condition:
			If saveEveryNIterations IsNot Nothing Then
				If saveEveryNIterSinceLast Then
					'Consider last saved model when deciding whether to save
					Dim lastSaveIter As Long = (If(lastCheckpoint_Conflict IsNot Nothing, lastCheckpoint_Conflict.getIteration(), startIter))
					If iteration - lastSaveIter >= saveEveryNIterations Then
						saveCheckpoint(model)
						Return
					End If
				Else
					'Same every N iterations, regardless of saving time
					If iteration > 0 AndAlso iteration Mod saveEveryNIterations = 0 Then
						saveCheckpoint(model)
						Return
					End If
				End If
			End If

			'Check time saving condition:
			Dim time As Long = DateTimeHelper.CurrentUnixTimeMillis()
			If saveEveryUnit IsNot Nothing Then
				If saveEverySinceLast Then
					'Consider last saved when deciding whether to save
					Dim lastSaveTime As Long = (If(lastCheckpoint_Conflict IsNot Nothing, lastCheckpoint_Conflict.getTimestamp(), startTime))
					If (time - lastSaveTime) >= saveEveryMs Then
						saveCheckpoint(model)
						Return
					End If
				Else
					'Save periodically, regardless of when last model was saved
					Dim lastSave As Long = (If(lastSaveEveryMsNoSinceLast IsNot Nothing, lastSaveEveryMsNoSinceLast, startTime))
					If (time - lastSave) > saveEveryMs Then
						saveCheckpoint(model)
						lastSaveEveryMsNoSinceLast = time
						Return
					End If
				End If
			End If
		End Sub

		Private Sub saveCheckpoint(ByVal model As Model)
			Try
				saveCheckpointHelper(model)
			Catch e As Exception
				Throw New Exception("Error saving checkpoint", e)
			End Try
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void saveCheckpointHelper(org.deeplearning4j.nn.api.Model model) throws Exception
		Private Sub saveCheckpointHelper(ByVal model As Model)
			If Not checkpointRecordFile.exists() Then
				checkpointRecordFile.createNewFile()
				write(Checkpoint.FileHeader & vbLf, checkpointRecordFile)
			End If

			lastCheckpointNum += 1
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: Checkpoint c = new Checkpoint(++lastCheckpointNum, System.currentTimeMillis(), getIter(model), getEpoch(model), getModelType(model), null);
			Dim c As New Checkpoint(lastCheckpointNum, DateTimeHelper.CurrentUnixTimeMillis(), getIter(model), getEpoch(model), getModelType(model), Nothing)
			FileName = c

			ModelSerializer.writeModel(model, New File(rootDir, c.getFilename()), True)

			Dim s As String = c.toFileString()
			write(s & vbLf, checkpointRecordFile)

			If logSaving Then
				log.info("Model checkpoint saved: epoch {}, iteration {}, path: {}", c.getEpoch(), c.getIteration(), (New File(rootDir, c.getFilename())).getPath())
			End If
			Me.lastCheckpoint_Conflict = c


			'Finally: determine if we should delete some old models...
			If keepMode = Nothing OrElse keepMode = KeepMode.ALL Then
				Return
			ElseIf keepMode = KeepMode.LAST Then
				Dim checkpoints As IList(Of Checkpoint) = availableCheckpoints()
				Dim iter As IEnumerator(Of Checkpoint) = checkpoints.GetEnumerator()
				Do While checkpoints.Count > keepLast
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
					Dim toRemove As Checkpoint = iter.next()
					Dim f As File = getFileForCheckpoint(toRemove)
					f.delete()
'JAVA TO VB CONVERTER TODO TASK: .NET enumerators are read-only:
					iter.remove()
				Loop
			Else
				'Keep mode: last N and every M
				For Each cp As Checkpoint In availableCheckpoints()
					If cp.getCheckpointNum() > 0 AndAlso (cp.getCheckpointNum()+1) Mod keepEvery = 0 Then
						'One of the "every M to keep" models
						Continue For
					ElseIf cp.getCheckpointNum() > lastCheckpointNum - keepLast Then 'Example: latest is 5, keep last 2 -> keep checkpoints 4 and 5
						'One of last N to keep
						Continue For
					End If
					'Otherwise: delete file
					Dim f As File = getFileForCheckpoint(cp)
					f.delete()
				Next cp
			End If
		End Sub

		Private Shared WriteOnly Property FileName As Checkpoint
			Set(ByVal c As Checkpoint)
				Dim filename As String = getFileName(c.getCheckpointNum(), c.getModelType())
				c.setFilename(filename)
			End Set
		End Property

		Private Shared Function getFileName(ByVal checkpointNum As Integer, ByVal modelType As String) As String
			Return "checkpoint_" & checkpointNum & "_" & modelType & ".zip"
		End Function

		Private Shared Function write(ByVal str As String, ByVal f As File) As String
			Try
				If Not f.exists() Then
					f.createNewFile()
				End If
				Files.append(str, f, Charset.defaultCharset())
			Catch e As IOException
				Throw New Exception(e)
			End Try
			Return str
		End Function

		Protected Friend Shared Function getIter(ByVal model As Model) As Integer
			If TypeOf model Is MultiLayerNetwork Then
				Return DirectCast(model, MultiLayerNetwork).LayerWiseConfigurations.getIterationCount()
			ElseIf TypeOf model Is ComputationGraph Then
				Return DirectCast(model, ComputationGraph).Configuration.getIterationCount()
			Else
				Return model.conf().getIterationCount()
			End If
		End Function

		Protected Friend Shared Function getEpoch(ByVal model As Model) As Integer
			If TypeOf model Is MultiLayerNetwork Then
				Return DirectCast(model, MultiLayerNetwork).LayerWiseConfigurations.EpochCount
			ElseIf TypeOf model Is ComputationGraph Then
				Return DirectCast(model, ComputationGraph).Configuration.getEpochCount()
			Else
				Return model.conf().getEpochCount()
			End If
		End Function

		Protected Friend Shared Function getModelType(ByVal model As Model) As String
			If model.GetType() = GetType(MultiLayerNetwork) Then
				Return "MultiLayerNetwork"
			ElseIf model.GetType() = GetType(ComputationGraph) Then
				Return "ComputationGraph"
			Else
				Return "Model"
			End If
		End Function

		''' <summary>
		''' List all available checkpoints. A checkpoint is 'available' if the file can be loaded. Any checkpoint files that
		''' have been automatically deleted (given the configuration) will not be returned here.
		''' </summary>
		''' <returns> List of checkpoint files that can be loaded </returns>
		Public Overridable Function availableCheckpoints() As IList(Of Checkpoint)
			If Not checkpointRecordFile.exists() Then
				Return java.util.Collections.emptyList()
			End If

			Return availableCheckpoints(rootDir)
		End Function

		''' <summary>
		''' List all available checkpoints. A checkpoint is 'available' if the file can be loaded. Any checkpoint files that
		''' have been automatically deleted (given the configuration) will not be returned here.
		''' Note that the checkpointInfo.txt file must exist, as this stores checkpoint information
		''' </summary>
		''' <returns> List of checkpoint files that can be loaded from the specified directory </returns>
		Public Shared Function availableCheckpoints(ByVal directory As File) As IList(Of Checkpoint)
			Dim checkpointRecordFile As New File(directory, "checkpointInfo.txt")
			Preconditions.checkState(checkpointRecordFile.exists(), "Could not find checkpoint record file at expected path %s", checkpointRecordFile.getAbsolutePath())

			Dim lines As IList(Of String)
			Try
					Using [is] As Stream = New BufferedInputStream(New FileStream(checkpointRecordFile, FileMode.Open, FileAccess.Read))
					lines = IOUtils.readLines([is])
					End Using
			Catch e As IOException
				Throw New Exception("Error loading checkpoint data from file: " & checkpointRecordFile.getAbsolutePath(), e)
			End Try

			Dim [out] As IList(Of Checkpoint) = New List(Of Checkpoint)(lines.Count - 1) 'Assume first line is header
			For i As Integer = 1 To lines.Count - 1
				Dim c As Checkpoint = Checkpoint.fromFileString(lines(i))
				If (New File(directory, c.getFilename())).exists() Then
					[out].Add(c)
				End If
			Next i
			Return [out]
		End Function

		''' <summary>
		''' Return the most recent checkpoint, if one exists - otherwise returns null </summary>
		''' <returns> Checkpoint </returns>
		Public Overridable Function lastCheckpoint() As Checkpoint
			If Not checkpointRecordFile.exists() Then
				Return Nothing
			End If
			Return lastCheckpoint(rootDir)
		End Function

		''' <summary>
		''' Return the most recent checkpoint, if one exists - otherwise returns null </summary>
		''' <param name="rootDir"> Root direcotry for the checkpoint files </param>
		''' <returns> Checkpoint </returns>
		Public Shared Function lastCheckpoint(ByVal rootDir As File) As Checkpoint
			Dim all As IList(Of Checkpoint) = availableCheckpoints(rootDir)
			If all.Count = 0 Then
				Return Nothing
			End If
			Return all(all.Count - 1)
		End Function

		''' <summary>
		''' Get the model file for the given checkpoint. Checkpoint model file must exist
		''' </summary>
		''' <param name="checkpoint"> Checkpoint to get the model file for </param>
		''' <returns> Model file for the checkpoint </returns>
'JAVA TO VB CONVERTER NOTE: The parameter checkpoint was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public Overridable Function getFileForCheckpoint(ByVal checkpoint_Conflict As Checkpoint) As File
			Return getFileForCheckpoint(checkpoint_Conflict.getCheckpointNum())
		End Function

		''' <summary>
		''' Get the model file for the given checkpoint number. Checkpoint model file must exist
		''' </summary>
		''' <param name="checkpointNum"> Checkpoint number to get the model file for </param>
		''' <returns> Model file for the checkpoint </returns>
		Public Overridable Function getFileForCheckpoint(ByVal checkpointNum As Integer) As File
			Return getFileForCheckpoint(rootDir, checkpointNum)
		End Function

		Public Shared Function getFileForCheckpoint(ByVal rootDir As File, ByVal checkpointNum As Integer) As File
			If checkpointNum < 0 Then
				Throw New System.ArgumentException("Invalid checkpoint number: " & checkpointNum)
			End If
			Dim f As File = Nothing
			For Each s As String In MODEL_TYPES
				f = New File(rootDir, getFileName(checkpointNum, s))
				If f.exists() Then
					Return f
				End If
			Next s
			Throw New System.InvalidOperationException("Model file for checkpoint " & checkpointNum & " does not exist")
		End Function

		''' <summary>
		''' Load a MultiLayerNetwork for the given checkpoint
		''' </summary>
		''' <param name="checkpoint"> Checkpoint model to load </param>
		''' <returns> The loaded model </returns>
'JAVA TO VB CONVERTER NOTE: The parameter checkpoint was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public Overridable Function loadCheckpointMLN(ByVal checkpoint_Conflict As Checkpoint) As MultiLayerNetwork
			Return loadCheckpointMLN(checkpoint_Conflict.getCheckpointNum())
		End Function

		''' <summary>
		''' Load a MultiLayerNetwork for the given checkpoint number
		''' </summary>
		''' <param name="checkpointNum"> Checkpoint model to load </param>
		''' <returns> The loaded model </returns>
		Public Overridable Function loadCheckpointMLN(ByVal checkpointNum As Integer) As MultiLayerNetwork
			Return loadCheckpointMLN(rootDir, checkpointNum)
		End Function

		''' <summary>
		''' Load a MultiLayerNetwork for the given checkpoint that resides in the specified root directory
		''' </summary>
		''' <param name="rootDir">    Root directory for the checkpoint </param>
		''' <param name="checkpoint"> Checkpoint model to load </param>
		''' <returns> The loaded model </returns>
'JAVA TO VB CONVERTER NOTE: The parameter checkpoint was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public Shared Function loadCheckpointMLN(ByVal rootDir As File, ByVal checkpoint_Conflict As Checkpoint) As MultiLayerNetwork
			Return loadCheckpointMLN(rootDir, checkpoint_Conflict.getCheckpointNum())
		End Function

		''' <summary>
		''' Load a MultiLayerNetwork for the given checkpoint number
		''' </summary>
		''' <param name="rootDir">       The directory that the checkpoint resides in </param>
		''' <param name="checkpointNum"> Checkpoint model to load </param>
		''' <returns> The loaded model </returns>
		Public Shared Function loadCheckpointMLN(ByVal rootDir As File, ByVal checkpointNum As Integer) As MultiLayerNetwork
			Dim f As File = getFileForCheckpoint(rootDir, checkpointNum)
			Try
				Return ModelSerializer.restoreMultiLayerNetwork(f, True)
			Catch e As IOException
				Throw New Exception(e)
			End Try
		End Function

		''' <summary>
		''' Load the last (most recent) checkpoint from the specified root directory </summary>
		''' <param name="rootDir"> Root directory to load checpoint from </param>
		''' <returns> MultiLayerNetwork for last checkpoint </returns>
		Public Shared Function loadLastCheckpointMLN(ByVal rootDir As File) As MultiLayerNetwork
			Dim last As Checkpoint = lastCheckpoint(rootDir)
			Return loadCheckpointMLN(rootDir, last)
		End Function

		''' <summary>
		''' Load a ComputationGraph for the given checkpoint
		''' </summary>
		''' <param name="checkpoint"> Checkpoint model to load </param>
		''' <returns> The loaded model </returns>
'JAVA TO VB CONVERTER NOTE: The parameter checkpoint was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public Overridable Function loadCheckpointCG(ByVal checkpoint_Conflict As Checkpoint) As ComputationGraph
			Return loadCheckpointCG(checkpoint_Conflict.getCheckpointNum())
		End Function

		''' <summary>
		''' Load a ComputationGraph for the given checkpoint from the specified root direcotry
		''' </summary>
		''' <param name="checkpoint"> Checkpoint model to load </param>
		''' <returns> The loaded model </returns>
'JAVA TO VB CONVERTER NOTE: The parameter checkpoint was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public Shared Function loadCheckpointCG(ByVal rootDir As File, ByVal checkpoint_Conflict As Checkpoint) As ComputationGraph
			Return loadCheckpointCG(rootDir, checkpoint_Conflict.getCheckpointNum())
		End Function

		''' <summary>
		''' Load a ComputationGraph for the given checkpoint
		''' </summary>
		''' <param name="checkpointNum"> Checkpoint model number to load </param>
		''' <returns> The loaded model </returns>
		Public Overridable Function loadCheckpointCG(ByVal checkpointNum As Integer) As ComputationGraph
			Return loadCheckpointCG(rootDir, checkpointNum)
		End Function

		''' <summary>
		''' Load a ComputationGraph for the given checkpoint that resides in the specified root directory
		''' </summary>
		''' <param name="rootDir">       Directory that the checkpoint resides in </param>
		''' <param name="checkpointNum"> Checkpoint model number to load </param>
		''' <returns> The loaded model </returns>
		Public Shared Function loadCheckpointCG(ByVal rootDir As File, ByVal checkpointNum As Integer) As ComputationGraph
			Dim f As File = getFileForCheckpoint(rootDir, checkpointNum)
			Try
				Return ModelSerializer.restoreComputationGraph(f, True)
			Catch e As IOException
				Throw New Exception(e)
			End Try
		End Function

		''' <summary>
		''' Load the last (most recent) checkpoint from the specified root directory </summary>
		''' <param name="rootDir"> Root directory to load checpoint from </param>
		''' <returns> ComputationGraph for last checkpoint </returns>
		Public Shared Function loadLastCheckpointCG(ByVal rootDir As File) As ComputationGraph
			Dim last As Checkpoint = lastCheckpoint(rootDir)
			Return loadCheckpointCG(rootDir, last)
		End Function

		Public Class Builder

			Friend rootDir As File
			Friend keepMode As KeepMode
'JAVA TO VB CONVERTER NOTE: The field keepLast was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend keepLast_Conflict As Integer
			Friend keepEvery As Integer
'JAVA TO VB CONVERTER NOTE: The field logSaving was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend logSaving_Conflict As Boolean = True
'JAVA TO VB CONVERTER NOTE: The field deleteExisting was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend deleteExisting_Conflict As Boolean = False

'JAVA TO VB CONVERTER NOTE: The field saveEveryNEpochs was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend saveEveryNEpochs_Conflict As Integer?
'JAVA TO VB CONVERTER NOTE: The field saveEveryNIterations was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend saveEveryNIterations_Conflict As Integer?
			Friend saveEveryNIterSinceLast As Boolean
			Friend saveEveryAmount As Long?
			Friend saveEveryUnit As TimeUnit
			Friend saveEverySinceLast As Boolean

			''' <param name="rootDir"> Root directory to save models to </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder(@NonNull String rootDir)
			Public Sub New(ByVal rootDir As String)
				Me.New(New File(rootDir))
			End Sub

			''' <param name="rootDir"> Root directory to save models to </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder(@NonNull File rootDir)
			Public Sub New(ByVal rootDir As File)
				Me.rootDir = rootDir
			End Sub

			''' <summary>
			''' Save a model at the end of every epoch
			''' </summary>
			Public Overridable Function saveEveryEpoch() As Builder
				Return saveEveryNEpochs(1)
			End Function

			''' <summary>
			''' Save a model at the end of every N epochs
			''' </summary>
			Public Overridable Function saveEveryNEpochs(ByVal n As Integer) As Builder
				Me.saveEveryNEpochs_Conflict = n
				Return Me
			End Function

			''' <summary>
			''' Save a model every N iterations
			''' </summary>
			Public Overridable Function saveEveryNIterations(ByVal n As Integer) As Builder
				Return saveEveryNIterations(n, False)
			End Function

			''' <summary>
			''' Save a model every N iterations (if sinceLast == false), or if N iterations have passed since
			''' the last model vas saved (if sinceLast == true)
			''' </summary>
			Public Overridable Function saveEveryNIterations(ByVal n As Integer, ByVal sinceLast As Boolean) As Builder
				Me.saveEveryNIterations_Conflict = n
				Me.saveEveryNIterSinceLast = sinceLast
				Return Me
			End Function

			''' <summary>
			''' Save a model periodically
			''' </summary>
			''' <param name="amount">   Quantity of the specified time unit </param>
			''' <param name="timeUnit"> Time unit </param>
			Public Overridable Function saveEvery(ByVal amount As Long, ByVal timeUnit As TimeUnit) As Builder
				Return saveEvery(amount, timeUnit, False)
			End Function

			''' <summary>
			''' Save a model periodically (if sinceLast == false), or if the specified amount of time has elapsed since
			''' the last model was saved (if sinceLast == true)
			''' </summary>
			''' <param name="amount">   Quantity of the specified time unit </param>
			''' <param name="timeUnit"> Time unit </param>
			Public Overridable Function saveEvery(ByVal amount As Long, ByVal timeUnit As TimeUnit, ByVal sinceLast As Boolean) As Builder
				Me.saveEveryAmount = amount
				Me.saveEveryUnit = timeUnit
				Me.saveEverySinceLast = sinceLast
				Return Me
			End Function

			''' <summary>
			''' Keep all model checkpoints - i.e., don't delete any. Note that this is the default.
			''' </summary>
			Public Overridable Function keepAll() As Builder
				Me.keepMode = KeepMode.ALL
				Return Me
			End Function

			''' <summary>
			''' Keep only the last N most recent model checkpoint files. Older checkpoints will automatically be deleted. </summary>
			''' <param name="n"> Number of most recent checkpoints to keep </param>
			Public Overridable Function keepLast(ByVal n As Integer) As Builder
				If n <= 0 Then
					Throw New System.ArgumentException("Number of model files to keep should be > 0 (got: " & n & ")")
				End If
				Me.keepMode = KeepMode.LAST
				Me.keepLast_Conflict = n
				Return Me
			End Function

			''' <summary>
			''' Keep the last N most recent model checkpoint files, <i>and</i> every M checkpoint files.<br>
			''' For example: suppose you save every 100 iterations, for 2050 iteration, and use keepLastAndEvery(3,5).
			''' This means after 2050 iterations you would have saved 20 checkpoints - some of which will be deleted.
			''' Those remaining in this example: iterations 500, 1000, 1500, 1800, 1900, 2000. </summary>
			''' <param name="nLast">  Most recent checkpoints to keep </param>
			''' <param name="everyN"> Every N checkpoints to keep (regardless of age) </param>
			Public Overridable Function keepLastAndEvery(ByVal nLast As Integer, ByVal everyN As Integer) As Builder
				If nLast <= 0 Then
					Throw New System.ArgumentException("Most recent number of model files to keep should be > 0 (got: " & nLast & ")")
				End If
				If everyN <= 0 Then
					Throw New System.ArgumentException("Every n model files to keep should be > 0 (got: " & everyN & ")")
				End If

				Me.keepMode = KeepMode.LAST_AND_EVERY
				Me.keepLast_Conflict = nLast
				Me.keepEvery = everyN
				Return Me
			End Function

			''' <summary>
			''' If true (the default) log a message every time a model is saved
			''' </summary>
			''' <param name="logSaving"> Whether checkpoint saves should be logged or not     </param>
'JAVA TO VB CONVERTER NOTE: The parameter logSaving was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function logSaving(ByVal logSaving_Conflict As Boolean) As Builder
				Me.logSaving_Conflict = logSaving_Conflict
				Return Me
			End Function

			''' <summary>
			''' If the checkpoint listener is set to save to a non-empty directory, should the CheckpointListener-related
			''' content be deleted?<br>
			''' This is disabled by default (and instead, an exception will be thrown if existing data is found)<br>
			''' WARNING: Be careful when enabling this, as it deletes all saved checkpoint models in the specified directory!
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter deleteExisting was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function deleteExisting(ByVal deleteExisting_Conflict As Boolean) As Builder
				Me.deleteExisting_Conflict = deleteExisting_Conflict
				Return Me
			End Function

			Public Overridable Function build() As CheckpointListener
				If saveEveryNEpochs_Conflict Is Nothing AndAlso saveEveryAmount Is Nothing AndAlso saveEveryNIterations_Conflict Is Nothing Then
					Throw New System.InvalidOperationException("Cannot construct listener: no models will be saved (must use at least" & " one of: save every N epochs, every N iterations, or every T time periods)")
				End If

				Return New CheckpointListener(Me)
			End Function
		End Class
	End Class

End Namespace