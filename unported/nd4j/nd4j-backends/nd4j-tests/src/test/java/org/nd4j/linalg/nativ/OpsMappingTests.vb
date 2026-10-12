Imports System
Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports DifferentialFunction = org.nd4j.autodiff.functions.DifferentialFunction
Imports FlatBuffersMapper = org.nd4j.autodiff.samediff.serde.FlatBuffersMapper
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports BaseBroadcastOp = org.nd4j.linalg.api.ops.BaseBroadcastOp
Imports BaseIndexAccumulation = org.nd4j.linalg.api.ops.BaseIndexAccumulation
Imports BaseReduceFloatOp = org.nd4j.linalg.api.ops.BaseReduceFloatOp
Imports BaseScalarOp = org.nd4j.linalg.api.ops.BaseScalarOp
Imports BaseTransformSameOp = org.nd4j.linalg.api.ops.BaseTransformSameOp
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports Op = org.nd4j.linalg.api.ops.Op
Imports Variance = org.nd4j.linalg.api.ops.impl.summarystats.Variance
Imports BaseRandomOp = org.nd4j.linalg.api.ops.random.BaseRandomOp
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports org.nd4j.common.primitives
Imports NativeOpsHolder = org.nd4j.nativeblas.NativeOpsHolder
Imports Reflections = org.reflections.Reflections
Imports SubTypesScanner = org.reflections.scanners.SubTypesScanner
Imports ClasspathHelper = org.reflections.util.ClasspathHelper
Imports ConfigurationBuilder = org.reflections.util.ConfigurationBuilder
Imports FilterBuilder = org.reflections.util.FilterBuilder

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

Namespace org.nd4j.linalg.nativ


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NativeTag public class OpsMappingTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class OpsMappingTests
		Inherits BaseNd4jTestWithBackends

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return 360000L 'Can be very slow on some CI machines (PPC)
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLegacyOpsMapping(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLegacyOpsMapping(ByVal backend As Nd4jBackend)
			Nd4j.create(1)

			Dim str As val = NativeOpsHolder.Instance.getDeviceNativeOps().getAllOperations().replaceAll("simdOps::","").replaceAll("randomOps::","")

			Dim missing As val = New List(Of String)()

			'parsing individual groups first

			Dim groups As val = str.split(">>")
			For Each group As val In groups
				Dim line As val = group.split(" ")
				Dim bt As val = Convert.ToInt32(line(0)).byteValue()
				Dim ops As val = line(1).Split("<<")

				Dim type As val = FlatBuffersMapper.getTypeFromByte(bt)
				Dim list As val = getOperations(type)

				For Each op As val In ops
					Dim args As val = op.split(":")
					Dim hash As val = Convert.ToInt64(args(0))
					Dim opNum As val = Convert.ToInt64(args(1))
					Dim name As val = args(2)

					'log.info("group: {}; hash: {}; name: {};", SameDiff.getTypeFromByte(bt), hash, name);
					Dim needle As val = New Operation(If(type = Op.Type.CUSTOM, -1, opNum), name.toLowerCase())
					If Not opMapped(list, needle) Then
						missing.add(type.ToString() & " " & name)
					End If

				Next op
			Next group

			If missing.size() > 0 Then

				log.info("{} ops missing!", missing.size())
				log.info("{}", missing)
				'assertTrue(false);
			End If
		End Sub

		Protected Friend Overridable Function opMapped(ByVal haystack As IList(Of Operation), ByVal needle As Operation) As Boolean
			For Each c As val In haystack
				If needle.First = -1L Then
					If c.getSecond().equalsIgnoreCase(needle.Second) Then
						Return True
					End If
				Else
					If c.getFirst().longValue() = needle.First Then
						Return True
					End If
				End If
			Next c

			Return False
		End Function

		Protected Friend Overridable Sub addOperation(ByVal clazz As Type, ByVal list As IList(Of Operation))
			If Modifier.isAbstract(clazz.getModifiers()) OrElse clazz.IsInterface Then
				Return
			End If

			Try
				Dim node As DifferentialFunction = System.Activator.CreateInstance(clazz)
				If TypeOf node Is DynamicCustomOp Then
					list.Add(New Operation(-1L, node.opName().ToLower()))
					list.Add(New Operation(-1L, node.tensorflowName().ToLower()))
				Else
					Dim op As val = New Operation(Convert.ToInt64(node.opNum()), node.opName())
					list.Add(op)
				End If
			Catch e As System.NotSupportedException
				'
			Catch e As NoOpNameFoundException
				'
			Catch e As InstantiationException
				'
			Catch e As Exception
				log.info("Failed on [{}]", clazz.Name)
				Throw New Exception(e)
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected java.util.List<Operation> getOperations(@NonNull Op.Type type)
		Protected Friend Overridable Function getOperations(ByVal type As Op.Type) As IList(Of Operation)
			Dim list As val = New List(Of Operation)()

			Dim f As New Reflections((New ConfigurationBuilder()).filterInputsBy((New FilterBuilder()).include(FilterBuilder.prefix("org.nd4j.*")).exclude("^(?!.*\.class$).*$")).setUrls(ClasspathHelper.forPackage("org.nd4j")).setScanners(New SubTypesScanner()))


			Select Case type
				Case SUMMARYSTATS
					Dim clazzes As ISet(Of Type) = f.getSubTypesOf(GetType(Variance))

					For Each clazz As Type In clazzes
						addOperation(clazz, list)
					Next clazz
				Case RANDOM
					Dim clazzes As ISet(Of Type) = f.getSubTypesOf(GetType(BaseRandomOp))

					For Each clazz As Type In clazzes
						addOperation(clazz, list)
					Next clazz
				Case INDEXREDUCE
					Dim clazzes As ISet(Of Type) = f.getSubTypesOf(GetType(BaseIndexAccumulation))

					For Each clazz As Type In clazzes
						addOperation(clazz, list)
					Next clazz
				Case REDUCE3, REDUCE_FLOAT
					Dim clazzes As ISet(Of Type) = f.getSubTypesOf(GetType(BaseReduceFloatOp))

					For Each clazz As Type In clazzes
						addOperation(clazz, list)
					Next clazz
				Case BROADCAST
					Dim clazzes As ISet(Of Type) = f.getSubTypesOf(GetType(BaseBroadcastOp))

					For Each clazz As Type In clazzes
						addOperation(clazz, list)
					Next clazz
				Case SCALAR
					Dim clazzes As ISet(Of Type) = f.getSubTypesOf(GetType(BaseScalarOp))

					For Each clazz As Type In clazzes
						addOperation(clazz, list)
					Next clazz
				Case PAIRWISE, TRANSFORM_SAME
					Dim clazzes As ISet(Of Type) = f.getSubTypesOf(GetType(BaseTransformSameOp))

					For Each clazz As Type In clazzes
						addOperation(clazz, list)
					Next clazz
				Case CUSTOM
					Dim clazzes As ISet(Of Type) = f.getSubTypesOf(GetType(DynamicCustomOp))

					For Each clazz As Type In clazzes
						If clazz.Name.Equals("dynamiccustomop", StringComparison.OrdinalIgnoreCase) Then
							Continue For
						End If

						addOperation(clazz, list)
					Next clazz
				'default:
				'    throw new ND4JIllegalStateException("Unknown operation type: " + type);
			End Select


			log.info("Group: {}; List size: {}", type, list.size())

			Return list
		End Function


		<Serializable>
		Protected Friend Class Operation
			Inherits Pair(Of Long, String)

			Protected Friend Sub New(ByVal opNum As Long?, ByVal name As String)
				MyBase.New(opNum, name)
			End Sub

			Public Overrides Function Equals(ByVal o As Object) As Boolean
				If Not (TypeOf o Is Operation) Then
					Return False
				End If

				Dim op As Operation = DirectCast(o, Operation)

				Return op.key.Equals(Me.key)
			End Function
		End Class
	End Class

End Namespace