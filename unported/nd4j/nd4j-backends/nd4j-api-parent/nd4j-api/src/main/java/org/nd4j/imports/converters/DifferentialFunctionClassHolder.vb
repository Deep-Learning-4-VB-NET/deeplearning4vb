Imports System
Imports System.Collections.Generic
Imports System.Reflection
Imports Getter = lombok.Getter
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports DifferentialFunction = org.nd4j.autodiff.functions.DifferentialFunction
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException
Imports OnnxDescriptorParser = org.nd4j.imports.descriptors.onnx.OnnxDescriptorParser
Imports OpDescriptor = org.nd4j.imports.descriptors.onnx.OpDescriptor
Imports TensorflowDescriptorParser = org.nd4j.imports.descriptors.tensorflow.TensorflowDescriptorParser
Imports org.nd4j.linalg.api.ops
Imports org.nd4j.linalg.api.ops.impl.controlflow.compat
Imports ExternalErrorsFunction = org.nd4j.linalg.api.ops.impl.layers.ExternalErrorsFunction
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports OpDef = org.tensorflow.framework.OpDef

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

Namespace org.nd4j.imports.converters


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class DifferentialFunctionClassHolder
	Public Class DifferentialFunctionClassHolder
		Private nodeConverters As IDictionary(Of String, DifferentialFunction) = ImportClassMapping.getOpNameMapping()
		Private tensorFlowNames As IDictionary(Of String, DifferentialFunction) = ImportClassMapping.getTFOpMappingFunctions()
		Private onnxNames As IDictionary(Of String, DifferentialFunction) = ImportClassMapping.getOnnxOpMappingFunctions()
		Private customOpHashToClass As IDictionary(Of Long, Type) = New Dictionary(Of Long, Type)()
		Private customOpHashToClasses As IDictionary(Of Long, IDictionary(Of String, Type)) = New Dictionary(Of Long, IDictionary(Of String, Type))() 'Only contains ops with 1 hash to multiple classes
'JAVA TO VB CONVERTER NOTE: The field missingOps was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private missingOps_Conflict As IList(Of String) = New List(Of String)()

		Private onnxOpDescriptors As IDictionary(Of String, OpDescriptor)
		Private tensorflowOpDescriptors As IDictionary(Of String, OpDef)
		Private fieldsForFunction As IDictionary(Of String, IDictionary(Of String, System.Reflection.FieldInfo))

		Private Shared ReadOnly fieldNamesOpsIgnore As ISet(Of String) = New LinkedHashSetAnonymousInnerClass()

		Private Class LinkedHashSetAnonymousInnerClass
			Inherits LinkedHashSet(Of String)

			Public Sub New()

				add("extraArgs")
				add("arrayInitialized")
				add("log")
				add("inputArguments")
				add("outputArguments")
				add("outputShapes")
				add("outputVariables")
				add("tArguments")
				add("iArguments")
				add("bArguments")
				add("dArguments")
				add("hash")
				add("opName")
				add("sameDiff")
				add("ownName")
			End Sub

		End Class
		'When determining fields/properties, where should we terminate the search?
		'We don't wan to include every single field from every single superclass
		Private Shared ReadOnly classesToIgnore As ISet(Of Type) = New HashSet(Of Type)(java.util.Arrays.asList(Of Type)(GetType(Object)))

		Private Shared ReadOnly classFieldsToIgnore As IDictionary(Of Type, ISet(Of String)) = New Dictionary(Of Type, ISet(Of String))()
		Shared Sub New()
			classFieldsToIgnore(GetType(BaseOp)) = New HashSet(Of String)(java.util.Arrays.asList("x", "y", "z", "n", "numProcessed", "xVertexId", "yVertexId", "zVertexId", "extraArgz"))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private int countTotalTfOps;
		Private countTotalTfOps As Integer
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private int countTotalMappedOps;
		Private countTotalMappedOps As Integer

'JAVA TO VB CONVERTER NOTE: The field INSTANCE was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared INSTANCE_Conflict As New DifferentialFunctionClassHolder()

		''' <summary>
		''' Get the fields for a given <seealso cref="DifferentialFunction"/> </summary>
		''' <param name="function"> the function to get the fields for </param>
		''' <returns> the fields for a given function </returns>
		Public Overridable Function getFieldsForFunction(ByVal [function] As DifferentialFunction) As IDictionary(Of String, System.Reflection.FieldInfo)
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			If Not fieldsForFunction.ContainsKey([function].GetType().FullName) Then
				Return java.util.Collections.emptyMap()
			End If
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			Return fieldsForFunction([function].GetType().FullName)
		End Function

		''' <summary>
		''' Get the op definition of a given
		''' tensorflow op.
		''' 
		''' Note that if the name does not exist,
		''' an <seealso cref="ND4JIllegalStateException"/> will be thrown </summary>
		''' <param name="name"> the name of the op </param>
		''' <returns> the op definition for a given op </returns>
		Public Overridable Function getOpDefByTensorflowName(ByVal name As String) As OpDef
			If Not tensorflowOpDescriptors.ContainsKey(name) Then
				Throw New ND4JIllegalStateException("No op found with name " & name)
			End If

			Return tensorflowOpDescriptors(name)
		End Function

		''' <summary>
		''' Get the op definition of a given
		''' onnx op
		''' Note that if the name does not exist,
		''' an <seealso cref="ND4JIllegalStateException"/>
		''' will be thrown. </summary>
		''' <param name="name"> the name of the op </param>
		''' <returns> the op definition for a given op </returns>
		Public Overridable Function getOpDescriptorForOnnx(ByVal name As String) As OpDescriptor
			If Not onnxOpDescriptors.ContainsKey(name) Then
				Throw New ND4JIllegalStateException("No op found with name " & name)
			End If

			Return onnxOpDescriptors(name)
		End Function

		''' <summary>
		''' Get the </summary>
		''' <param name="tensorflowName">
		''' @return </param>
		Public Overridable Function getOpWithTensorflowName(ByVal tensorflowName As String) As DifferentialFunction
			Return tensorFlowNames(tensorflowName)
		End Function

		Public Overridable Function getOpWithOnnxName(ByVal onnxName As String) As DifferentialFunction
			Return onnxNames(onnxName)
		End Function

		Private Sub New()
			fieldsForFunction = New LinkedHashMap(Of String, IDictionary(Of String, System.Reflection.FieldInfo))()

			For Each df As DifferentialFunction In ImportClassMapping.getOpNameMapping().Values
				If df Is Nothing OrElse df.opName() Is Nothing Then
					Continue For
				End If
				Try
					'accumulate the field names for a given function
					'this is mainly used in import
					Dim fieldNames As IDictionary(Of String, System.Reflection.FieldInfo) = New LinkedHashMap(Of String, System.Reflection.FieldInfo)()
					Dim current As Type = df.GetType()
					Dim fields As val = New List(Of System.Reflection.FieldInfo)()
					Dim isFirst As Boolean = True

					Do While current.BaseType IsNot Nothing AndAlso Not classesToIgnore.Contains(current.BaseType)

						If df.ConfigProperties AndAlso isFirst Then

							Dim fieldName As String = df.configFieldName()

							If fieldName Is Nothing Then
								fieldName = "config"
							End If

							Dim configField As System.Reflection.FieldInfo = Nothing
							Try
								configField = current.getDeclaredField(fieldName)
							Catch e As NoSuchFieldException
								Dim currentConfig As Type = current.BaseType

								' find a config field in superclasses
								Do While currentConfig.BaseType IsNot Nothing
									Try
										configField = currentConfig.getDeclaredField(fieldName)
										Exit Do
									Catch e2 As NoSuchFieldException
										currentConfig = currentConfig.BaseType
									End Try
								Loop
							End Try

							If configField Is Nothing Then
								Continue Do
							End If

							Dim configFieldClass As val = configField.getType()

							For Each field As val In configFieldClass.getDeclaredFields()
								If Not Modifier.isStatic(field.getModifiers()) AndAlso Not fieldNamesOpsIgnore.Contains(field.getName()) AndAlso (Not classFieldsToIgnore.ContainsKey(current) OrElse Not classFieldsToIgnore(current).Contains(field.getName())) Then
									fields.add(field)
									field.setAccessible(True)
									If fieldNames.ContainsKey(field.getName()) Then
										Throw New System.InvalidOperationException("Field with name " & field.getName() & " exists for multiple classes: " & fieldNames(field.getName()).getDeclaringClass().getName() & " and " & field.getDeclaringClass().getName())
									End If
									fieldNames(field.getName()) = field
								End If
							Next field
						Else
							For Each field As System.Reflection.FieldInfo In current.GetFields(BindingFlags.DeclaredOnly Or BindingFlags.Public Or BindingFlags.NonPublic Or BindingFlags.Static Or BindingFlags.Instance)
								If Not Modifier.isStatic(field.getModifiers()) AndAlso Not fieldNamesOpsIgnore.Contains(field.getName()) AndAlso (Not classFieldsToIgnore.ContainsKey(current) OrElse Not classFieldsToIgnore(current).Contains(field.getName())) Then
									fields.add(field)
									field.setAccessible(True)
									If fieldNames.ContainsKey(field.getName()) Then
										Throw New System.InvalidOperationException("Field with name " & field.getName() & " exists for multiple classes: " & fieldNames(field.getName()).getDeclaringClass().getName() & " and " & field.getDeclaringClass().getName())
									End If
									fieldNames(field.getName()) = field
								End If
							Next field
						End If

						' do something with current's fields
						current = CType(current.BaseType, Type)
						isFirst = False

					Loop

'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					fieldsForFunction(df.GetType().FullName) = fieldNames
				Catch e As NoOpNameFoundException
					log.trace("Skipping function  " & df.GetType())
				Catch e As Exception
					Throw New Exception(e)
				End Try
			Next df

			'get the op descriptors for onnx and tensorflow
			'this is used when validating operations
			Try
				tensorflowOpDescriptors = TensorflowDescriptorParser.opDescs()
				onnxOpDescriptors = OnnxDescriptorParser.onnxOpDescriptors()
			Catch e As Exception
				Throw New Exception(e)
			End Try


			Dim map As val = New Dictionary(Of )(Nd4j.Executioner.getCustomOperations())
			Dim set As val = map.keySet()
			set.removeAll(nodeConverters.Keys)
			CType(missingOps_Conflict, List(Of String)).AddRange(set)
			missingOps_Conflict.Sort()
			'log.debug("Missing " + set.size() + " ops!");

			countTotalTfOps = tensorflowOpDescriptors.Count

			'Work out total number of TF ops mapped
			Dim tfMappedOps As ISet(Of String) = New HashSet(Of String)()
			For Each df As DifferentialFunction In nodeConverters.Values
				Try
					Dim tfNames() As String = df.tensorflowNames()
					Collections.addAll(tfMappedOps, tfNames)
				Catch e As NoOpNameFoundException
					'Ignore
				End Try
			Next df
			countTotalMappedOps = tfMappedOps.Count

			'Get custom ops - map from hash to class
			Dim descriptorMap As IDictionary(Of String, CustomOpDescriptor) = Nd4j.Executioner.getCustomOperations()
			Dim multiClassHashes As ISet(Of Long) = New HashSet(Of Long)()
			For Each e As KeyValuePair(Of String, CustomOpDescriptor) In descriptorMap.SetOfKeyValuePairs()
				Dim name As String = e.Key
				Dim df As DifferentialFunction = getInstance(name)

				If df Is Nothing Then
					'Can be no class for 2 reasons:
					'(a) op name aliases
					'(b) libnd4j ops with no corresponding ND4J op class
					Continue For
				End If

				If Not df.GetType().IsAssignableFrom(GetType(CustomOp)) Then
					'Not a custom op class
					Continue For
				End If

				Dim h As Long = e.Value.getHash()
				If customOpHashToClass.ContainsKey(h) Then
					'One op hash mapped to multiple classes
					multiClassHashes.Add(h)
				End If
				customOpHashToClass(e.Value.getHash()) = df.GetType()
			Next e

			For Each e As KeyValuePair(Of String, CustomOpDescriptor) In descriptorMap.SetOfKeyValuePairs()
				Dim h As Long = e.Value.getHash()
				If multiClassHashes.Contains(h) Then
					If Not customOpHashToClasses.ContainsKey(h) Then
						customOpHashToClasses(h) = New Dictionary(Of String, Type)()
					End If
					Dim m As IDictionary(Of String, Type) = customOpHashToClasses(h)
					Dim name As String = e.Key
					Dim df As DifferentialFunction = getInstance(name)
					If df Is Nothing Then
						Continue For
					End If
					m(e.Key) = df.GetType()
				End If
			Next e
		End Sub


		''' <summary>
		'''*
		''' Returns the missing onnx ops
		''' @return
		''' </summary>
		Public Overridable Function missingOnnxOps() As ISet(Of String)
			Dim copy As ISet(Of String) = New HashSet(Of String)(onnxOpDescriptors.Keys)
			copy.RemoveAll(onnxNames.Keys)
			Return copy
		End Function


		''' <summary>
		'''*
		''' Returns the missing tensorflow ops
		''' @return
		''' </summary>
		Public Overridable Function missingTensorflowOps() As ISet(Of String)
			Dim copy As ISet(Of String) = New HashSet(Of String)(tensorflowOpDescriptors.Keys)
			copy.RemoveAll(tensorFlowNames.Keys)
			Return copy
		End Function

		''' <summary>
		''' Returns the missing ops
		''' for c++ vs java.
		''' @return
		''' </summary>
		Public Overridable Function missingOps() As IList(Of String)
			Return missingOps_Conflict
		End Function

		''' 
		''' <param name="name">
		''' @return </param>
		Public Overridable Function hasName(ByVal name As String) As Boolean
			Return nodeConverters.ContainsKey(name)
		End Function


		Public Overridable Function opNames() As ISet(Of String)
			Return nodeConverters.Keys
		End Function

		''' 
		''' <param name="name">
		''' @return </param>
		Public Overridable Function getInstance(ByVal name As String) As DifferentialFunction
			Return nodeConverters(name)
		End Function

		Public Overridable Function customOpClassForHashAndName(ByVal customOpHash As Long, ByVal name As String) As Type
			Select Case name
				Case Enter.OP_NAME
					Return GetType(Enter)
				Case [Exit].OP_NAME
					Return GetType([Exit])
				Case NextIteration.OP_NAME
					Return GetType(NextIteration)
				Case Merge.OP_NAME
					Return GetType(Merge)
				Case Switch.OP_NAME
					Return GetType(Switch)
				Case LoopCond.OP_NAME
					Return GetType(LoopCond)
				Case ExternalErrorsFunction.OP_NAME
					Return GetType(ExternalErrorsFunction)
				Case Else
					If customOpHashToClasses.ContainsKey(customOpHash) Then
						Return customOpHashToClasses(customOpHash)(name)
					ElseIf customOpHashToClass.ContainsKey(customOpHash) Then
						Return customOpHashToClass(customOpHash)
					Else
						Throw New System.InvalidOperationException("No op known for hash: " & customOpHash)
					End If
			End Select

		End Function

		Public Shared ReadOnly Property Instance As DifferentialFunctionClassHolder
			Get
				Return INSTANCE_Conflict
			End Get
		End Property

		Public Overridable ReadOnly Property TensorFlowNames As IDictionary(Of String, DifferentialFunction)
			Get
				Return Collections.unmodifiableMap(tensorFlowNames)
			End Get
		End Property
	End Class

End Namespace