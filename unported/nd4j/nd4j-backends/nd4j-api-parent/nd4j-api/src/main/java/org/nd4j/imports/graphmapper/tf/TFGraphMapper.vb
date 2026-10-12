Imports System
Imports System.Collections.Generic
Imports System.IO
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports FileUtils = org.apache.commons.io.FileUtils
Imports IOUtils = org.apache.commons.io.IOUtils
Imports DifferentialFunction = org.nd4j.autodiff.functions.DifferentialFunction
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports VariableType = org.nd4j.autodiff.samediff.VariableType
Imports SameDiffOp = org.nd4j.autodiff.samediff.internal.SameDiffOp
Imports Variable = org.nd4j.autodiff.samediff.internal.Variable
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DifferentialFunctionClassHolder = org.nd4j.imports.converters.DifferentialFunctionClassHolder
Imports AttributeAdapter = org.nd4j.imports.descriptors.properties.AttributeAdapter
Imports PropertyMapping = org.nd4j.imports.descriptors.properties.PropertyMapping
Imports org.nd4j.imports.graphmapper.tf.tensors
Imports TFTensorMappers = org.nd4j.imports.graphmapper.tf.tensors.TFTensorMappers
Imports TFImportOverride = org.nd4j.imports.tensorflow.TFImportOverride
Imports TFOpImportFilter = org.nd4j.imports.tensorflow.TFOpImportFilter
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Merge = org.nd4j.linalg.api.ops.impl.controlflow.compat.Merge
Imports Floats = org.nd4j.shade.guava.primitives.Floats
Imports Ints = org.nd4j.shade.guava.primitives.Ints
Imports Message = org.nd4j.shade.protobuf.Message
Imports TextFormat = org.nd4j.shade.protobuf.TextFormat
Imports org.tensorflow.framework
Imports ListOrderedSet = org.apache.commons.collections4.set.ListOrderedSet

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

Namespace org.nd4j.imports.graphmapper.tf


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class TFGraphMapper
	Public Class TFGraphMapper
		<Obsolete("Use static methods - <seealso cref=""importGraph(File)""/> etc")>
		Public Shared ReadOnly Property Instance As TFGraphMapper
			Get
				Return New TFGraphMapper()
			End Get
		End Property

		''' <summary>
		''' Import a frozen TensorFlow protobuf (.pb) file from the specified file
		''' </summary>
		''' <param name="f"> Frozen TensorFlow model pb file to import </param>
		''' <returns> Imported graph </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.nd4j.autodiff.samediff.SameDiff importGraph(@NonNull File f)
		Public Shared Function importGraph(ByVal f As File) As SameDiff
			Return importGraph(f, Nothing, Nothing)
		End Function

		''' <summary>
		''' Import a frozen TensorFlow protobuf (.pb) file from the specified file, with optional overrides
		''' </summary>
		''' <param name="f">              Frozen TensorFlow model pb file to import </param>
		''' <param name="importOverride"> Optional import override for specific ops, keyed by op name </param>
		''' <param name="opFilter">       Optional filter - ops to exclude/ignore </param>
		''' <returns> Imported graph </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.nd4j.autodiff.samediff.SameDiff importGraph(@NonNull File f, Map<String, org.nd4j.imports.tensorflow.TFImportOverride> importOverride, org.nd4j.imports.tensorflow.TFOpImportFilter opFilter)
		Public Shared Function importGraph(ByVal f As File, ByVal importOverride As IDictionary(Of String, TFImportOverride), ByVal opFilter As TFOpImportFilter) As SameDiff
			Preconditions.checkState(f.exists(), "File does not exist: %s", f)
			Try
					Using [is] As Stream = New BufferedInputStream(New FileStream(f, FileMode.Open, FileAccess.Read))
					Return importGraph([is], importOverride, opFilter)
					End Using
			Catch e As IOException
				Throw New Exception(e)
			End Try
		End Function

		''' <summary>
		''' Import a frozen TensorFlow protobuf (.pb) file, via an input stream
		''' </summary>
		''' <param name="is"> Stream for a frozen TensorFlow model pb file to import </param>
		''' <returns> Imported graph </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.nd4j.autodiff.samediff.SameDiff importGraph(@NonNull InputStream is)
		Public Shared Function importGraph(ByVal [is] As Stream) As SameDiff
			Return importGraph([is], Nothing, Nothing)
		End Function

		''' <summary>
		''' Import a frozen TensorFlow protobuf file in text format (.pb.txt) file via an input stream, with optional overrides
		''' </summary>
		''' <param name="is">             Stream for a frozen TensorFlow model pb file to import </param>
		''' <param name="importOverride"> Optional import override for specific ops, keyed by op name </param>
		''' <param name="opFilter">       Optional filter - ops to exclude/ignore </param>
		''' <returns> Imported graph </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.nd4j.autodiff.samediff.SameDiff importGraphTxt(@NonNull InputStream is, Map<String, org.nd4j.imports.tensorflow.TFImportOverride> importOverride, org.nd4j.imports.tensorflow.TFOpImportFilter opFilter)
		Public Shared Function importGraphTxt(ByVal [is] As Stream, ByVal importOverride As IDictionary(Of String, TFImportOverride), ByVal opFilter As TFOpImportFilter) As SameDiff
			Dim tfGraph As GraphDef
			Try
				Dim builder As Message.Builder = GraphDef.newBuilder()
				Dim content As String = IOUtils.toString([is], StandardCharsets.UTF_8)
				TextFormat.getParser().merge(content, builder)
				tfGraph = CType(builder.build(), GraphDef)
			Catch e As IOException
				Throw New Exception(e)
			End Try

			Return importGraph(tfGraph, importOverride, opFilter)
		End Function

		''' <summary>
		''' Import a frozen TensorFlow protobuf (.pb) file via an input stream, with optional overrides
		''' </summary>
		''' <param name="is">             Stream for a frozen TensorFlow model pb file to import </param>
		''' <param name="importOverride"> Optional import override for specific ops, keyed by op name </param>
		''' <param name="opFilter">       Optional filter - ops to exclude/ignore </param>
		''' <returns> Imported graph </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.nd4j.autodiff.samediff.SameDiff importGraph(@NonNull InputStream is, Map<String, org.nd4j.imports.tensorflow.TFImportOverride> importOverride, org.nd4j.imports.tensorflow.TFOpImportFilter opFilter)
		Public Shared Function importGraph(ByVal [is] As Stream, ByVal importOverride As IDictionary(Of String, TFImportOverride), ByVal opFilter As TFOpImportFilter) As SameDiff
			Dim tfGraph As GraphDef
			Try
				tfGraph = GraphDef.parseFrom([is])
			Catch e As IOException
				Throw New Exception(e)
			End Try

			Return importGraph(tfGraph, importOverride, opFilter)
		End Function

		''' <summary>
		''' Import a TensorFlow model from a GraphDef
		''' </summary>
		''' <param name="tfGraph"> TensorFlow model GraphDef </param>
		''' <returns> Imported model </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.nd4j.autodiff.samediff.SameDiff importGraph(@NonNull GraphDef tfGraph)
		Public Shared Function importGraph(ByVal tfGraph As GraphDef) As SameDiff
			Return importGraph(tfGraph, Nothing, Nothing)
		End Function

		''' <summary>
		''' Import a TensorFlow model from a GraphDef, with optional import overrides
		''' </summary>
		''' <param name="tfGraph">        TensorFlow model GraphDef </param>
		''' <param name="importOverride"> Optional import override for specific ops, keyed by op name </param>
		''' <param name="opFilter">       Optional filter - ops to exclude/ignore </param>
		''' <returns> Imported model </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.nd4j.autodiff.samediff.SameDiff importGraph(@NonNull GraphDef tfGraph, Map<String, org.nd4j.imports.tensorflow.TFImportOverride> importOverride, org.nd4j.imports.tensorflow.TFOpImportFilter opFilter)
		Public Shared Function importGraph(ByVal tfGraph As GraphDef, ByVal importOverride As IDictionary(Of String, TFImportOverride), ByVal opFilter As TFOpImportFilter) As SameDiff

	'        
	'        First, build an in-memory representation of the graph that allows us to build the graph incrementally
	'        If we can build the graph incrementally, we can make sure that the added variables are set up with the correct
	'        datatype and (once implemented) greedy shape inference
	'         

			Dim variablesAdded As IList(Of String) = New List(Of String)()
			Dim opsAdded As IList(Of String) = New List(Of String)()
			Dim opsImported As IList(Of String) = New List(Of String)()
			Dim opsRemoved As IList(Of String) = New List(Of String)()
			Dim availableToAddSet As ISet(Of String) = New HashSet(Of String)() 'TODO maybe unnecessary?
			Dim availableToAdd As New LinkedList(Of NodeDef)()

			Dim remainingNodes As IDictionary(Of String, NodeDef) = New Dictionary(Of String, NodeDef)() 'All other nodes, not in availableToAdd

			Dim nodeInputTo As IDictionary(Of String, ListOrderedSet(Of String)) = New Dictionary(Of String, ListOrderedSet(Of String))() ' For op x -> y, x is key, y is value. Note that these are OP names not VARIABLE names

			Dim nNodes As Integer = tfGraph.getNodeCount()

			'First, add any constants, placeholders, and zero-input ops
			Dim sd As SameDiff = SameDiff.create()
			For i As Integer = 0 To nNodes - 1
				Dim nd As NodeDef = tfGraph.getNode(i)
				Dim op As String = nd.getOp()
				Dim name As String = nd.getName()

				Dim nInputs As Integer = nd.getInputCount()

				If "Const".Equals(op) OrElse "Placeholder".Equals(op) OrElse nInputs = 0 Then
					availableToAdd.AddLast(nd)
					availableToAddSet.Add(name)
				Else
					remainingNodes(name) = nd
					For [in] As Integer = 0 To nInputs - 1
						Dim inOpName As String = stripControl(nd.getInput([in]))
						inOpName = stripVarSuffix(inOpName)

						If Not nodeInputTo.ContainsKey(inOpName) Then
							nodeInputTo(inOpName) = New ListOrderedSet(Of String)()
						End If

						nodeInputTo(inOpName).add(name)
					Next [in]
				End If
			Next i

			Dim mergeOpsPostProcess As IDictionary(Of String, String) = New Dictionary(Of String, String)()

			'Go through ops in order, and add to the graph
			Dim constControlDeps As IDictionary(Of String, IList(Of String)) = New Dictionary(Of String, IList(Of String))() 'Key: constant name. Value: control dependencies
			Do While availableToAdd.Count > 0
				Dim nd As NodeDef = availableToAdd.RemoveFirst()
				Dim name As String = nd.getName()
				Dim opName As String = nd.getOp()
				Dim nIn As Integer = nd.getInputCount()

				availableToAddSet.remove(name)

				log.trace("Adding operation to graph: {} (name={})", opName, name)
				opsAdded.Add(opName & "," & name)
				Dim skipCase As Boolean = False
				If opFilter IsNot Nothing AndAlso opFilter.skipOp(nd, sd, nd.getAttrMap(), tfGraph) Then
					log.debug("Skipping op {} of type {} due to op filter", name, opName)
					'Don't continue at this point - we still need to process what this feeds into...
					skipCase = True
				Else
					If importOverride Is Nothing OrElse Not importOverride.ContainsKey(name) Then
						'Standard case
						If "Const".Equals(opName) Then
							'Get array, create a constant
							Dim tfTensor As TensorProto = nd.getAttrOrThrow("value").getTensor()
							Dim m As TFTensorMapper = TFTensorMappers.newMapper(tfTensor)
							Dim arr As INDArray = m.toNDArray()
							sd.constant(name, arr)
							Dim inputCount As Integer = nd.getInputCount()
							If inputCount > 0 Then
								'Very likely control dependency. i.e., "we must execute op X before the constant is really available to be used"
								Dim l As IList(Of String) = New List(Of String)(inputCount)
								For i As Integer = 0 To inputCount - 1
									Dim n As String = nd.getInput(i)
									If Not isControlDep(n) Then
										Throw New System.InvalidOperationException("Found non-control dependency input """ & n & """ for constant """ & name & """")
									End If
									Dim n2 As String = stripControl(n)
									l.Add(n2)
								Next i
								constControlDeps(name) = l
							End If
						ElseIf "Placeholder".Equals(opName) OrElse "PlaceholderWithDefault".Equals(opName) Then
							'TODO support the "WithDefault" array

							Dim attrMap As IDictionary(Of String, AttrValue) = nd.getAttrMap()
							Dim shapeAvailable As Boolean = attrMap.ContainsKey("shape")
							Dim shape() As Long
							If shapeAvailable Then
								Dim shapeProto As TensorShapeProto = attrMap("shape").getShape()
								shape = shapeFromShapeProto(shapeProto)
							Else
								'Some placeholders don't have any shape restrictions - i.e., accept anything...
								shape = Nothing
							End If


							Dim tfDtype As org.tensorflow.framework.DataType = attrMap("dtype").getType()
							Dim dt As org.nd4j.linalg.api.buffer.DataType = convertType(tfDtype)
							sd.placeHolder(name, dt, shape)
						Else
	'                        
	'                        Normal ops. Process in the following order:
	'                        1. Create the op instance
	'                        2. Add op to graph
	'                        3. Import from TF (to set attributes)
	'                        4. Calculate output dtypes
	'                        5. Create and add output variables to graph
	'
	'                        Note: one constraint on this order is that some ops import modify the graph structure.
	'                        Notable example: concat op - it removes the axis op and converts the value to an iArg
	'                        https://github.com/eclipse/deeplearning4j/issues/8285
	'                         
							Dim dfInstance As DifferentialFunction = DifferentialFunctionClassHolder.Instance.getOpWithTensorflowName(opName)
							Preconditions.checkState(dfInstance IsNot Nothing, "Could not find class for TF Ops: %s", opName)

							Dim df As DifferentialFunction
							Try
								df = System.Activator.CreateInstance(dfInstance.GetType())
							Catch t As Exception
								'Should never happen because function was already created via no-arg constructor earlier
								Throw New Exception(t)
							End Try
							df.setSameDiff(sd)
							df.setOwnName(name)

							'Process inputs
							Dim inNames As IList(Of String) = New List(Of String)(nIn)
							Dim controlDeps As IList(Of String) = Nothing
							For i As Integer = 0 To nIn - 1
								Dim origInName As String = nd.getInput(i)
								Dim inName As String = stripControl(origInName)

								If inName.EndsWith(":0", StringComparison.Ordinal) Then
									'Strip ":0" suffix. Some ops can depend on placeholders, like "image_tensor:0" but in SameDiff this is a variable called "image_tensor"
									inName = inName.Substring(0, inName.Length-2)
								End If

								Dim isControlDep As Boolean = TFGraphMapper.isControlDep(origInName)
								If isControlDep Then
									If controlDeps Is Nothing Then
										controlDeps = New List(Of String)()
									End If
									controlDeps.Add(inName)
								End If

								If Not isControlDep Then
									inNames.Add(inName)
								End If

								'Update Variable.inputsForOp for all variables that feed into this op
								' Such variables must have already been created, given we process in order
								Dim v As Variable = sd.getVariables().get(inName)

								If v Is Nothing AndAlso TypeOf df Is Merge Then
									'Edge case for import - we allow merge ops to be added before both inputs are available
									'This is to break the cycles in loops, otherwise we can't process anything in order
									mergeOpsPostProcess(df.getOwnName()) = inName
									Continue For
								End If

								If Not isControlDep AndAlso (v.getInputsForOp() Is Nothing OrElse Not v.getInputsForOp().contains(name)) Then
									'May already be present - for example, add(x,x)
									If v.getInputsForOp() Is Nothing Then
										v.setInputsForOp(New List(Of String)())
									End If
									v.getInputsForOp().add(name)
								ElseIf isControlDep Then
									If v.getControlDepsForOp() Is Nothing Then
										v.setControlDepsForOp(New List(Of String)())
									End If
									If Not v.getControlDepsForOp().contains(name) Then
										v.getControlDepsForOp().add(name)
									End If
								End If
							Next i

							'Create SameDiffOp instance and add to graph
							Dim op As SameDiffOp = SameDiffOp.builder().name(name).op(df).inputsToOp(inNames).controlDeps(controlDeps).build()
							sd.getOps().put(name, op)


							Dim attrMap As IDictionary(Of String, AttrValue) = nd.getAttrMap()
							df.initFromTensorFlow(nd, sd, attrMap, tfGraph) 'TODO REMOVE TFGRAPH ENTIRELY FROM THIS CALL - it encourages hacky and really brittle stuff like input array to attribute conversion

							'DType calculate for output variables (set/correct if necessary)
							Dim newInNames As IList(Of String) = sd.getOps().get(name).getInputsToOp() 'Just in case import has modified this, like for concat case
							Dim newInDtypes As IList(Of org.nd4j.linalg.api.buffer.DataType) = New List(Of org.nd4j.linalg.api.buffer.DataType)(newInNames.Count)
							If TypeOf df Is Merge Then
								'Merge op: as noted elsewhere, we allow merge to be processed when only one of the inputs is available
								' to break cycles for loops
								'We know that Merge op has the restriction of the same datatype for both inputs, so we'll
								Dim v1 As SDVariable = sd.getVariable(newInNames(0))
								Dim v2 As SDVariable = sd.getVariable(newInNames(1))
								Dim dt1 As org.nd4j.linalg.api.buffer.DataType = (If(v1 Is Nothing, v2.dataType(), v1.dataType()))
								Dim dt2 As org.nd4j.linalg.api.buffer.DataType = (If(v2 Is Nothing, v1.dataType(), v2.dataType()))
								newInDtypes.Add(dt1)
								newInDtypes.Add(dt2)
							Else
								For Each s As String In newInNames
									Dim v As SDVariable = sd.getVariable(s)
									newInDtypes.Add(v.dataType())
								Next s
							End If

							Dim outDTypes As IList(Of org.nd4j.linalg.api.buffer.DataType) = df.calculateOutputDataTypes(newInDtypes)
							Dim outSDVars(outDTypes.Count - 1) As SDVariable
							Dim outVars(outDTypes.Count - 1) As Variable
							Dim outNames As IList(Of String) = New List(Of String)(outDTypes.Count)

							'Create output variables and add to graph
							For i As Integer = 0 To outDTypes.Count - 1
								Dim dt As org.nd4j.linalg.api.buffer.DataType = outDTypes(i)
								Dim varName As String = name & (If(i = 0, "", ":" & i))
								outSDVars(i) = sd.var(varName, VariableType.ARRAY, Nothing, dt, DirectCast(Nothing, Long()))
								outNames.Add(varName)

								outVars(i) = Variable.builder().name(varName).variable(outSDVars(i)).inputsForOp(Nothing).controlDepsForOp(Nothing).controlDepsForVar(Nothing).outputOfOp(name).build()

								sd.getVariables().put(varName, outVars(i))
								log.trace("Added variable to graph: {} (output of op {})", varName, name)
								variablesAdded.Add(varName & "," & name)
							Next i
							sd.getOps().get(name).setOutputsOfOp(outNames)

							log.trace("Imported op: {} (name={})", opName, name)
							opsImported.Add(opName & "," & name)
						End If
					Else
						'Import override case
						Dim o As TFImportOverride = importOverride(name)

						log.debug("Importing op {} using override {}", opName, importOverride)

						'First, get inputs:
						Dim inputs As IList(Of SDVariable) = New List(Of SDVariable)(nIn)
						Dim controlDeps As IList(Of SDVariable) = Nothing
						For i As Integer = 0 To nIn - 1
							Dim inName As String = nd.getInput(i)
							Dim controlDep As Boolean = isControlDep(inName)

							Dim v As SDVariable = sd.getVariable(name)

							If controlDep Then
								If controlDeps Is Nothing Then
									controlDeps = New List(Of SDVariable)()
								End If
								controlDeps.Add(v)
							Else
								inputs.Add(v)
							End If

							o.initFromTensorFlow(inputs, controlDeps, nd, sd, nd.getAttrMap(), tfGraph)
						Next i
					End If
				End If


				'Now that we have just added an op (or variable) - check what this feeds into, and see what we can now process
				' as a result
				If nodeInputTo.ContainsKey(name) Then
					Dim set As ISet(Of String) = nodeInputTo(name)
					For Each nextOp As String In set
						Dim nextOpDef As NodeDef = remainingNodes(nextOp)
						If nextOpDef Is Nothing Then
							If sd.getOps().containsKey(nextOp) Then
								'Already processed this.
								'Almost certainly the close of a loop - like NextIteration -> Merge case
								Continue For
							End If
							'Should never happen
							Throw New System.InvalidOperationException("Could not find op definition for op to import: " & nextOp)
						End If

						Dim nInNext As Integer = nextOpDef.getInputCount()
						Dim allAlreadyInGraph As Boolean = True
						Dim nonControlSeenCount As Integer = 0
						For i As Integer = 0 To nInNext - 1
							Dim s As String = nextOpDef.getInput(i)
							Dim inName As String = stripControl(nextOpDef.getInput(i))

							If inName.EndsWith(":0", StringComparison.Ordinal) Then
								'Strip ":0" suffix. Some ops can depend on placeholders, like "image_tensor:0" but in SameDiff this is a variable called "image_tensor"
								inName = inName.Substring(0, inName.Length-2)
							End If

	'                        log.info("Input: {}, {}", s, inName);

							If Not sd.hasVariable(inName) AndAlso Not skipCase Then
	'                            log.info("Not found: {} for op {}", inName, nextOpDef.getName());
								allAlreadyInGraph = False
								Exit For
							ElseIf Not isControlDep(s) Then
								nonControlSeenCount += 1
							End If
						Next i

						'Merge ops are an edge case. We'll allow these to be executed with just ONE input, to break
						' the cycle in loops. In loops, generally we have (Enter, NextIteration) -> Merge, which
						' of course can't be done if we strictly require all inputs to be available
						Dim mergeCase As Boolean = (nonControlSeenCount > 0 AndAlso "Merge".Equals(nextOpDef.getOp()))

						If allAlreadyInGraph OrElse mergeCase Then
							'Can process this op, add it to the queue for processing
							If Not availableToAddSet.Contains(nextOp) Then
								'Avoid processing same op multiple times, for repeated inputs to one op, etc
								availableToAdd.AddLast(nextOpDef)
								availableToAddSet.Add(nextOp)
								log.trace("Added to processing queue: {} (name={})", nextOpDef.getOp(), nextOp)
							End If
						End If
					Next nextOp
				End If

				'Finally, remove the just processed op from remainingNodes map:
				remainingNodes.Remove(name)
				opsRemoved.Add(name)
			Loop

			'Post process the control dependencies, if any (done after because dependencies may not exist when imported)
			For Each e As KeyValuePair(Of String, IList(Of String)) In constControlDeps.SetOfKeyValuePairs()
				Dim varName As String = e.Key
				Dim cdOpNames As IList(Of String) = e.Value
				sd.getVariables().get(varName).setControlDeps(cdOpNames)

				For Each s As String In cdOpNames
					Dim sdo As SameDiffOp = sd.getOps().get(s)
					If sdo.getControlDepFor() Is Nothing Then
						sdo.ControlDepFor = New List(Of String)()
					End If
					Dim l As IList(Of String) = sdo.getControlDepFor()
					If Not l.Contains(s) Then
						l.Add(varName)
					End If
				Next s
			Next e

			'Post process the merge ops - all we are missing is a Variable.getInputsForOp().add(mergeOpName);
			For Each e As KeyValuePair(Of String, String) In mergeOpsPostProcess.SetOfKeyValuePairs()
				Dim v As Variable = sd.getVariables().get(e.Value)
				If v.getInputsForOp() Is Nothing Then
					v.setInputsForOp(New List(Of String)())
				End If
				v.getInputsForOp().add(e.Key)
			Next e

			Preconditions.checkState(remainingNodes.Count = 0, "%s Unprocessed nodes: %s", remainingNodes.Count, remainingNodes.Keys)
			Try
				FileUtils.writeLines(New File("variables-added-old.txt"),variablesAdded)
				FileUtils.writeLines(New File("ops-imported-old.txt"),opsImported)
				FileUtils.writeLines(New File("ops-added-old.txt"),opsAdded)
				FileUtils.writeLines(New File("ops-removed-old.txt"),opsRemoved)

			Catch e As IOException
				Console.WriteLine(e.ToString())
				Console.Write(e.StackTrace)
			End Try
			log.trace("Variables added " & variablesAdded)
			log.trace("Ops imported " & opsImported)
			log.trace("Ops added" & opsAdded)
			log.trace("Ops removed " & opsRemoved)
			Return sd
		End Function


		''' <summary>
		''' Get the shape from a TensorShapeProto
		''' </summary>
		''' <param name="tensorShapeProto"> Shape </param>
		''' <returns> Shape as long[] </returns>
		Private Shared Function shapeFromShapeProto(ByVal tensorShapeProto As TensorShapeProto) As Long()
			Dim shape((tensorShapeProto.getDimList().size()) - 1) As Long
			For i As Integer = 0 To shape.Length - 1
				shape(i) = tensorShapeProto.getDim(i).getSize()
			Next i

			Return shape
		End Function

		''' <summary>
		''' Convert from TF proto datatype to ND4J datatype
		''' </summary>
		''' <param name="tfType"> TF datatype </param>
		''' <returns> ND4J datatype </returns>
		Public Shared Function convertType(ByVal tfType As org.tensorflow.framework.DataType) As org.nd4j.linalg.api.buffer.DataType
			Select Case tfType
				Case DT_DOUBLE
					Return org.nd4j.linalg.api.buffer.DataType.DOUBLE
				Case DT_FLOAT
					Return org.nd4j.linalg.api.buffer.DataType.FLOAT
				Case DT_HALF
					Return org.nd4j.linalg.api.buffer.DataType.HALF
				Case DT_BFLOAT16
					Return org.nd4j.linalg.api.buffer.DataType.BFLOAT16
				Case DT_INT8
					Return org.nd4j.linalg.api.buffer.DataType.BYTE
				Case DT_INT16
					Return org.nd4j.linalg.api.buffer.DataType.SHORT
				Case DT_INT32
					Return org.nd4j.linalg.api.buffer.DataType.INT
				Case DT_INT64
					Return org.nd4j.linalg.api.buffer.DataType.LONG
				Case DT_UINT8
					Return org.nd4j.linalg.api.buffer.DataType.UBYTE
				Case DT_STRING
					Return org.nd4j.linalg.api.buffer.DataType.UTF8
				Case DT_BOOL
					Return org.nd4j.linalg.api.buffer.DataType.BOOL

				Case Else
					Return org.nd4j.linalg.api.buffer.DataType.UNKNOWN
			End Select
		End Function

		''' <returns> True if the specified name represents a control dependency (starts with "^") </returns>
		Protected Friend Shared Function isControlDep(ByVal name As String) As Boolean
			Return name.StartsWith("^", StringComparison.Ordinal)
		End Function

		''' <returns> The specified name without the leading "^" character (if any) that appears for control dependencies </returns>
		Protected Friend Shared Function stripControl(ByVal name As String) As String
			If name.StartsWith("^", StringComparison.Ordinal) Then
				Return name.Substring(1)
			End If
			Return name
		End Function

		''' <summary>
		''' Remove the ":1" etc suffix for a variable name to get the op name
		''' </summary>
		''' <param name="varName"> Variable name </param>
		''' <returns> Variable name without any number suffix </returns>
		Protected Friend Shared Function stripVarSuffix(ByVal varName As String) As String
			If varName.matches(".*:\d+") Then
				Dim idx As Integer = varName.LastIndexOf(":"c)
				Dim ret As String = varName.Substring(0, idx)
				Return ret
			End If
			Return varName
		End Function

		''' <summary>
		''' Convert the tensor to an NDArray (if possible and if array is available)
		''' </summary>
		''' <param name="node"> Node to get NDArray from </param>
		''' <returns> NDArray </returns>
		Public Shared Function getNDArrayFromTensor(ByVal node As NodeDef) As INDArray
			'placeholder of some kind
			If Not node.getAttrMap().containsKey("value") Then
				Return Nothing
			End If

			Dim tfTensor As val = node.getAttrOrThrow("value").getTensor()
			Dim [out] As INDArray = mapTensorProto(tfTensor)
			Return [out]
		End Function

		''' <summary>
		''' Convert a TensorProto to an INDArray
		''' </summary>
		''' <param name="tfTensor"> Tensor proto </param>
		''' <returns> INDArray </returns>
		Public Shared Function mapTensorProto(ByVal tfTensor As TensorProto) As INDArray
			Dim m As TFTensorMapper = TFTensorMappers.newMapper(tfTensor)
			If m Is Nothing Then
				Throw New Exception("Not implemented datatype: " & tfTensor.getDtype())
			End If
			Dim [out] As INDArray = m.toNDArray()
			Return [out]
		End Function

		<Obsolete>
		Public Shared Function getNodeWithNameFromGraph(ByVal graph As GraphDef, ByVal name As String) As NodeDef
			Dim i As Integer = 0
			Do While i < graph.getNodeCount()
				Dim node As val = graph.getNode(i)
				If node.getName().Equals(name) Then
					Return node
				End If
				i += 1
			Loop

			Return Nothing
		End Function

		<Obsolete>
		Public Shared Function getArrayFrom(ByVal nodeDef As NodeDef, ByVal graph As GraphDef) As INDArray
			If nodeDef Is Nothing Then
				Return Nothing
			End If

			Return getNDArrayFromTensor(nodeDef)
		End Function

		''' <summary>
		''' Init a function's attributes
		''' </summary>
		''' <param name="mappedTfName">      the tensorflow name to pick (sometimes ops have multiple names </param>
		''' <param name="on">                the function to map </param>
		''' <param name="attributesForNode"> the attributes for the node </param>
		''' <param name="node"> </param>
		''' <param name="graph"> </param>
		''' @deprecated To be removed 
		<Obsolete("To be removed")>
		Public Shared Sub initFunctionFromProperties(ByVal mappedTfName As String, ByVal [on] As DifferentialFunction, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal node As NodeDef, ByVal graph As GraphDef)
			Dim properties As val = [on].mappingsForFunction()
			Dim tfProperties As val = properties.get(mappedTfName)
			Dim fields As val = DifferentialFunctionClassHolder.Instance.getFieldsForFunction([on])
			Dim attributeAdapters As val = [on].attributeAdaptersForFunction()

			' if there's no properties announced for this function - just return
			If tfProperties Is Nothing Then
				Return
			End If

			'Can't execute in just any order: sometimes there are dependencies between attribute mappings
			'For example, conv2d strides depend on data format -> need to map data format before mapping strides
			'Solution: map nodes without adapters before nodes with adapters. This doesn't guarantee we'll always be
			' mapping in the right order (for example, we might have adapter(x) depends on adapter(y)) but it should catch most cases
			Dim map As IDictionary(Of String, PropertyMapping)
			If attributeAdapters Is Nothing OrElse Not attributeAdapters.containsKey(mappedTfName) Then
				map = tfProperties
			Else
				map = New LinkedHashMap(Of String, PropertyMapping)()
				For Each e As KeyValuePair(Of String, PropertyMapping) In tfProperties.entrySet()
					If Not attributeAdapters.get(mappedTfName).containsKey(e.Key) Then
						'No adapter for this attribute
						map(e.Key) = e.Value
					End If
				Next e
				For Each e As KeyValuePair(Of String, PropertyMapping) In tfProperties.entrySet()
					If Not map.ContainsKey(e.Key) Then
						'Not added on first pass -> must have attribute mapper
						map(e.Key) = e.Value
					End If
				Next e
			End If

			For Each entry As KeyValuePair(Of String, PropertyMapping) In map.SetOfKeyValuePairs()
				Dim tfAttrName As val = entry.Value.getTfAttrName()
				Dim currentField As val = fields.get(entry.Key)

				Dim adapter As AttributeAdapter = Nothing
				If attributeAdapters IsNot Nothing AndAlso Not attributeAdapters.isEmpty() Then
					Dim mappers As val = attributeAdapters.get(mappedTfName)
					Dim adapterFor As val = mappers.get(entry.Key)
					adapter = adapterFor
				End If


				If tfAttrName IsNot Nothing Then
					If currentField Is Nothing Then
						Continue For
					End If

					If attributesForNode.ContainsKey(tfAttrName) Then
						Dim attr As val = attributesForNode(tfAttrName)
						Select Case attr.getValueCase()
							Case B
								If adapter IsNot Nothing Then
									adapter.mapAttributeFor(attr.getB(), currentField, [on])
								End If
							Case F
							Case FUNC
							Case S
								Dim setString As val = attr.getS().toStringUtf8()
								If adapter IsNot Nothing Then
									adapter.mapAttributeFor(setString, currentField, [on])
								Else
									[on].setValueFor(currentField, setString)
								End If
							Case I
								Dim setInt As val = CInt(Math.Truncate(attr.getI()))
								If adapter IsNot Nothing Then
									adapter.mapAttributeFor(setInt, currentField, [on])
								Else
									[on].setValueFor(currentField, setInt)
								End If
							Case SHAPE
								Dim shape As val = attr.getShape().getDimList()
								Dim dimsToSet(shape.size() - 1) As Integer
								For i As Integer = 0 To dimsToSet.Length - 1
									dimsToSet(i) = CInt(Math.Truncate(shape.get(i).getSize()))
								Next i

								If adapter IsNot Nothing Then
									adapter.mapAttributeFor(dimsToSet, currentField, [on])
								Else
									[on].setValueFor(currentField, dimsToSet)
								End If
							Case VALUE_NOT_SET
							Case PLACEHOLDER
							Case LIST
								Dim setList As val = attr.getList()
								If Not setList.getIList().isEmpty() Then
									Dim intList As val = Ints.toArray(setList.getIList())
									If adapter IsNot Nothing Then
										adapter.mapAttributeFor(intList, currentField, [on])
									Else
										[on].setValueFor(currentField, intList)
									End If
								ElseIf Not setList.getBList().isEmpty() Then
									Exit Select
								ElseIf Not setList.getFList().isEmpty() Then
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: lombok.val floats = org.nd4j.shade.guava.primitives.Floats.toArray((Collection<? extends Number>) setList.getFList());
									Dim floats As val = Floats.toArray(CType(setList.getFList(), ICollection(Of Number)))
									If adapter IsNot Nothing Then
										adapter.mapAttributeFor(floats, currentField, [on])
									Else
										[on].setValueFor(currentField, floats)
									End If
									Exit Select
								ElseIf Not setList.getFuncList().isEmpty() Then
									Exit Select
								ElseIf Not setList.getTensorList().isEmpty() Then
									Exit Select
								End If
							Case TENSOR
								Dim tensorToGet As val = TFGraphMapper.mapTensorProto(attr.getTensor())
								If adapter IsNot Nothing Then
									adapter.mapAttributeFor(tensorToGet, currentField, [on])
								Else
									[on].setValueFor(currentField, tensorToGet)
								End If
							Case TYPE
								If adapter IsNot Nothing Then
									adapter.mapAttributeFor(attr.getType(), currentField, [on])
								End If
						End Select
					End If
				ElseIf entry.Value.getTfInputPosition() IsNot Nothing Then


					Dim position As Integer = entry.Value.getTfInputPosition()
					If position < 0 Then
						position += node.getInputCount()
					End If

					Dim inputFromNode As val = TFGraphMapper.getNodeWithNameFromGraph(graph, node.getInput(position))
					Dim tensor As INDArray = If(inputFromNode IsNot Nothing, TFGraphMapper.getNDArrayFromTensor(inputFromNode), Nothing)
					If tensor Is Nothing Then
						tensor = [on].getSameDiff().getArrForVarName(getNodeName(node.getInput(position)))
					End If


					If tensor IsNot Nothing Then
						'use adapter instead of direct mapping just like above
						If adapter IsNot Nothing Then
							adapter.mapAttributeFor(tensor, currentField, [on])
						Else
							If currentField.getType().Equals(GetType(Integer())) Then
								[on].setValueFor(currentField, tensor.data().asInt())
							ElseIf currentField.getType().Equals(GetType(Double())) Then
								[on].setValueFor(currentField, tensor.data().asDouble())

							ElseIf currentField.getType().Equals(GetType(Single())) Then
								[on].setValueFor(currentField, tensor.data().asFloat())

							ElseIf currentField.getType().Equals(GetType(INDArray)) Then
								[on].setValueFor(currentField, tensor)
							ElseIf currentField.getType().Equals(GetType(Integer)) Then
								[on].setValueFor(currentField, tensor.getInt(0))
							ElseIf currentField.getType().Equals(GetType(Double)) Then
								[on].setValueFor(currentField, tensor.getDouble(0))
							ElseIf currentField.getType().Equals(GetType(Single)) Then
								[on].setValueFor(currentField, tensor.getFloat(0))
							End If
						End If
					End If
				End If
			Next entry
		End Sub

		''' <summary>
		''' Map a tensorflow node name
		''' to the samediff equivalent
		''' for import
		''' </summary>
		''' <param name="name"> the name to change </param>
		''' <returns> the input tensorflow name </returns>
		''' @deprecated To be removed 
		<Obsolete("To be removed")>
		Public Shared Function getNodeName(ByVal name As String) As String
			'tensorflow adds colons to the end of variables representing input index, this strips those off
			Dim ret As String = name
			If ret.StartsWith("^", StringComparison.Ordinal) Then
				ret = ret.Substring(1)
			End If
			If ret.EndsWith("/read", StringComparison.Ordinal) Then
				ret = ret.Replace("/read", "")
			End If
			If ret.EndsWith(":0", StringComparison.Ordinal) Then
				ret = ret.Substring(0, ret.Length - 2)
			End If
			Return ret
		End Function

		''' <summary>
		''' Determine if the node represents a variable node (based on op name)
		''' </summary>
		''' <param name="nodeDef"> Node to check if a variable </param>
		''' <returns> True if a variable node </returns>
		Public Shared Function isVariableNode(ByVal nodeDef As NodeDef) As Boolean
			Dim isVar As Boolean = nodeDef.getOp().StartsWith("VariableV") OrElse nodeDef.getOp().equalsIgnoreCase("const")
			Return isVar
		End Function

		''' <summary>
		''' Determine if the node is a placeholder
		''' </summary>
		''' <param name="nodeDef"> Node to check </param>
		''' <returns> True if the node is a placeholder </returns>
		Public Shared Function isPlaceHolder(ByVal nodeDef As NodeDef) As Boolean
			Return nodeDef.getOp().StartsWith("Placeholder")
		End Function
	End Class

End Namespace