Imports System.IO
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports Execution = org.junit.jupiter.api.parallel.Execution
Imports ExecutionMode = org.junit.jupiter.api.parallel.ExecutionMode
Imports Isolated = org.junit.jupiter.api.parallel.Isolated
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports WorkspaceConfiguration = org.nd4j.linalg.api.memory.conf.WorkspaceConfiguration
Imports AllocationPolicy = org.nd4j.linalg.api.memory.enums.AllocationPolicy
Imports LearningPolicy = org.nd4j.linalg.api.memory.enums.LearningPolicy
Imports ResetPolicy = org.nd4j.linalg.api.memory.enums.ResetPolicy
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports org.junit.jupiter.api.Assertions

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

Namespace org.nd4j.linalg.compression



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NativeTag @Tag(TagNames.COMPRESSION) @Isolated @Execution(ExecutionMode.SAME_THREAD) public class CompressionTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class CompressionTests
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCompressionDescriptorSerde(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCompressionDescriptorSerde(ByVal backend As Nd4jBackend)
			Dim descriptor As New CompressionDescriptor()
			descriptor.setCompressedLength(4)
			descriptor.setOriginalElementSize(4)
			descriptor.setNumberOfElements(4)
			descriptor.setCompressionAlgorithm("GZIP")
			descriptor.setOriginalLength(4)
			descriptor.setOriginalDataType(DataType.LONG)
			descriptor.setCompressionType(CompressionType.LOSSY)
			Dim toByteBuffer As ByteBuffer = descriptor.toByteBuffer()
			Dim fromByteBuffer As CompressionDescriptor = CompressionDescriptor.fromByteBuffer(toByteBuffer)
			assertEquals(descriptor, fromByteBuffer)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGzipInPlaceCompression(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGzipInPlaceCompression(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.create(New Single() {1f, 2f, 3f, 4f, 5f})
			Nd4j.Compressor.DefaultCompression = "GZIP"
			Nd4j.Compressor.compressi(array)
			assertTrue(array.Compressed)
			Nd4j.Compressor.decompressi(array)
			assertFalse(array.Compressed)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGzipCompression1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGzipCompression1(ByVal backend As Nd4jBackend)
			Dim array As INDArray = Nd4j.linspace(1, 10000, 20000, DataType.FLOAT)
			Dim exp As INDArray = array.dup()

			BasicNDArrayCompressor.Instance.DefaultCompression = "GZIP"

			Dim compr As INDArray = BasicNDArrayCompressor.Instance.compress(array)

			assertEquals(DataType.COMPRESSED, compr.data().dataType())

			Dim decomp As INDArray = BasicNDArrayCompressor.Instance.decompress(compr)

			assertEquals(exp, array)
			assertEquals(exp, decomp)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNoOpCompression1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNoOpCompression1(ByVal backend As Nd4jBackend)
			Nd4j.setDefaultDataTypes(DataType.FLOAT, DataType.FLOAT)
			Dim array As INDArray = Nd4j.linspace(1, 10000, 20000, DataType.FLOAT)
			Dim exp As INDArray = Nd4j.linspace(1, 10000, 20000, DataType.FLOAT)
			Dim mps As INDArray = Nd4j.linspace(1, 10000, 20000, DataType.FLOAT)

			BasicNDArrayCompressor.Instance.DefaultCompression = "NOOP"

			Dim compr As INDArray = BasicNDArrayCompressor.Instance.compress(array)

			assertEquals(DataType.COMPRESSED, compr.data().dataType())
			assertTrue(compr.Compressed)

			Dim decomp As INDArray = BasicNDArrayCompressor.Instance.decompress(compr)

			assertEquals(DataType.FLOAT, decomp.data().dataType())
			assertFalse(decomp.Compressed)
			assertFalse(TypeOf decomp.data() Is CompressedDataBuffer)
			assertFalse(TypeOf exp.data() Is CompressedDataBuffer)
			assertFalse(exp.Compressed)
			assertFalse(TypeOf array.data() Is CompressedDataBuffer)

			assertEquals(exp, decomp)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testJVMCompression3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testJVMCompression3(ByVal backend As Nd4jBackend)
			Nd4j.setDefaultDataTypes(DataType.FLOAT, DataType.FLOAT)
			Dim exp As INDArray = Nd4j.create(New Single() {1f, 2f, 3f, 4f, 5f}).reshape(ChrW(1), -1)

			BasicNDArrayCompressor.Instance.DefaultCompression = "NOOP"

			Dim compressed As INDArray = BasicNDArrayCompressor.Instance.compress(New Single() {1f, 2f, 3f, 4f, 5f})
			assertNotEquals(Nothing, compressed.data())
			assertNotEquals(Nothing, compressed.shapeInfoDataBuffer())
			assertTrue(compressed.Compressed)

			Dim decomp As INDArray = BasicNDArrayCompressor.Instance.decompress(compressed)

			assertEquals(exp, decomp)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Disabled @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testThresholdCompression0(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testThresholdCompression0(ByVal backend As Nd4jBackend)
			Dim initial As INDArray = Nd4j.rand(New Integer() {1, 150000000}, 119L)

			log.info("DTYPE: {}", Nd4j.dataType())

			Dim configuration As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(2 * 1024L * 1024L * 1024L).overallocationLimit(0).policyAllocation(AllocationPolicy.STRICT).policyLearning(LearningPolicy.NONE).policyReset(ResetPolicy.BLOCK_LEFT).build()


			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(configuration, "IIIA")
				Dim compressed As INDArray = Nd4j.Executioner.thresholdEncode(initial.dup(), 0.999)
			End Using

			Dim timeS As Long = 0
			For i As Integer = 0 To 99
				Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(configuration, "IIIA")
					Dim d As INDArray = initial.dup()
					Dim time1 As Long = System.nanoTime()
					Dim compressed As INDArray = Nd4j.Executioner.thresholdEncode(d, 0.999)
					Dim time2 As Long = System.nanoTime()
					timeS += (time2 - time1) \ 1000
				End Using
			Next i


			log.info("Elapsed time: {} us", (timeS) \ 100)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testThresholdCompression1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testThresholdCompression1(ByVal backend As Nd4jBackend)
			Dim initial As INDArray = Nd4j.create(New Single() {0.0f, 0.0f, 1e-3f, -1e-3f, 0.0f, 0.0f})
			Dim exp_0 As INDArray = Nd4j.create(DataType.FLOAT, 6)
			Dim exp_1 As INDArray = initial.dup()

			Dim compressor As NDArrayCompressor = Nd4j.Compressor.getCompressor("THRESHOLD")
			compressor.configure(1e-3)

			Dim compressed As INDArray = compressor.compress(initial)

			log.info("Initial array: {}", Arrays.toString(initial.data().asFloat()))

			Dim decompressed As INDArray = compressor.decompress(compressed)

			assertEquals(exp_1, decompressed)
			assertEquals(exp_0, initial)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testThresholdCompression2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testThresholdCompression2(ByVal backend As Nd4jBackend)
			Dim initial As INDArray = Nd4j.create(New Double() {1.0, 2.0, 0.0, 0.0, -1.0, -1.0})
			Dim exp_0 As INDArray = Nd4j.create(New Double() {1.0 - 1e-3, 2.0 - 1e-3, 0.0, 0.0, -1.0 + 1e-3, -1.0 + 1e-3})
			Dim exp_1 As INDArray = Nd4j.create(New Double() {1e-3, 1e-3, 0.0, 0.0, -1e-3, -1e-3})

			'Nd4j.getCompressor().getCompressor("THRESHOLD").configure(1e-3);
			'NDArray compressed = Nd4j.getCompressor().compress(initial, "THRESHOLD");
			Dim compressed As INDArray = Nd4j.Executioner.thresholdEncode(initial, 1e-3f)

			log.info("Initial array: {}", Arrays.toString(initial.data().asFloat()))

			assertEquals(exp_0, initial)

			Dim decompressed As INDArray = Nd4j.create(DataType.DOUBLE, initial.length())
			Nd4j.Executioner.thresholdDecode(compressed, decompressed)

			log.info("Decompressed array: {}", Arrays.toString(decompressed.data().asFloat()))

			assertEquals(exp_1, decompressed)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testThresholdCompression3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testThresholdCompression3(ByVal backend As Nd4jBackend)
			Dim initial As INDArray = Nd4j.create(New Double() {-1.0, -2.0, 0.0, 0.0, 1.0, 1.0})
			Dim exp_0 As INDArray = Nd4j.create(New Double() {-1.0 + 1e-3, -2.0 + 1e-3, 0.0, 0.0, 1.0 - 1e-3, 1.0 - 1e-3})
			Dim exp_1 As INDArray = Nd4j.create(New Double() {-1e-3, -1e-3, 0.0, 0.0, 1e-3, 1e-3})

			'Nd4j.getCompressor().getCompressor("THRESHOLD").configure(1e-3);
			Dim compressed As INDArray = Nd4j.Executioner.thresholdEncode(initial, 1e-3f)

			Dim copy As INDArray = compressed.unsafeDuplication()

			log.info("Initial array: {}", Arrays.toString(initial.data().asFloat()))

			assertEquals(exp_0, initial)

			Dim decompressed As INDArray = Nd4j.create(DataType.DOUBLE, initial.length())
			Nd4j.Executioner.thresholdDecode(compressed, decompressed)

			log.info("Decompressed array: {}", Arrays.toString(decompressed.data().asFloat()))

			assertEquals(exp_1, decompressed)

			Dim decompressed_copy As INDArray = Nd4j.create(DataType.DOUBLE, initial.length())
			Nd4j.Executioner.thresholdDecode(copy, decompressed_copy)

			assertFalse(decompressed Is decompressed_copy)
			assertEquals(decompressed, decompressed_copy)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testThresholdCompression4(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testThresholdCompression4(ByVal backend As Nd4jBackend)
			Dim initial As INDArray = Nd4j.create(New Double() {1e-4, -1e-4, 0.0, 0.0, 1e-4, -1e-4})
			Dim exp_0 As INDArray = initial.dup()


			'Nd4j.getCompressor().getCompressor("THRESHOLD").configure(1e-3);
			Dim compressed As INDArray = Nd4j.Executioner.thresholdEncode(initial, 1e-3f)


			log.info("Initial array: {}", Arrays.toString(initial.data().asFloat()))

			assertEquals(exp_0, initial)

			assertNull(compressed)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testThresholdCompression5(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testThresholdCompression5(ByVal backend As Nd4jBackend)
			Dim initial As INDArray = Nd4j.ones(10)
			Dim exp_0 As INDArray = initial.dup()

			Nd4j.Executioner.commit()

			'Nd4j.getCompressor().getCompressor("THRESHOLD").configure(1e-3);
			Dim compressed As INDArray = Nd4j.Executioner.thresholdEncode(initial, 1.0f, 3)

			assertEquals(7, compressed.data().length())

			assertNotEquals(exp_0, initial)

			assertEquals(7, initial.sumNumber().doubleValue(), 0.01)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testThresholdCompression5_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testThresholdCompression5_1(ByVal backend As Nd4jBackend)
			Dim initial As INDArray = Nd4j.ones(1000)
			Dim exp_0 As INDArray = initial.dup()

			Nd4j.Executioner.commit()

			'Nd4j.getCompressor().getCompressor("THRESHOLD").configure(1e-3);
			Dim compressed As INDArray = Nd4j.Executioner.thresholdEncode(initial, 1.0f, 100)

			assertEquals(104, compressed.data().length())

			assertNotEquals(exp_0, initial)

			assertEquals(900, initial.sumNumber().doubleValue(), 0.01)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testThresholdCompression6(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testThresholdCompression6(ByVal backend As Nd4jBackend)
			Dim initial As INDArray = Nd4j.create(New Double() {1.0, 2.0, 0.0, 0.0, -1.0, -1.0})
			Dim exp_0 As INDArray = Nd4j.create(New Double() {1.0 - 1e-3, 2.0 - 1e-3, 0.0, 0.0, -1.0 + 1e-3, -1.0 + 1e-3})
			Dim exp_1 As INDArray = Nd4j.create(New Double() {1e-3, 1e-3, 0.0, 0.0, -1e-3, -1e-3})
			Dim exp_2 As INDArray = Nd4j.create(New Double() {2e-3, 2e-3, 0.0, 0.0, -2e-3, -2e-3})

			'Nd4j.getCompressor().getCompressor("THRESHOLD").configure(1e-3);
			'NDArray compressed = Nd4j.getCompressor().compress(initial, "THRESHOLD");
			Dim compressed As INDArray = Nd4j.Executioner.thresholdEncode(initial, 1e-3f)

			log.info("Initial array: {}", Arrays.toString(initial.data().asFloat()))

			assertEquals(exp_0, initial)

			Dim decompressed As INDArray = Nd4j.create(DataType.DOUBLE, initial.length())
			Nd4j.Executioner.thresholdDecode(compressed, decompressed)

			log.info("Decompressed array: {}", Arrays.toString(decompressed.data().asFloat()))

			assertEquals(exp_1, decompressed)

			Nd4j.Executioner.thresholdDecode(compressed, decompressed)

			assertEquals(exp_2, decompressed)
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testThresholdSerialization1(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testThresholdSerialization1(ByVal backend As Nd4jBackend)
			Dim initial As INDArray = Nd4j.create(New Double() {-1.0, -2.0, 0.0, 0.0, 1.0, 1.0})
			Dim exp_0 As INDArray = Nd4j.create(New Double() {-1.0 + 1e-3, -2.0 + 1e-3, 0.0, 0.0, 1.0 - 1e-3, 1.0 - 1e-3})
			Dim exp_1 As INDArray = Nd4j.create(New Double() {-1e-3, -1e-3, 0.0, 0.0, 1e-3, 1e-3})

			'Nd4j.getCompressor().getCompressor("THRESHOLD").configure(1e-3);
			Dim compressed As INDArray = Nd4j.Executioner.thresholdEncode(initial, 1e-3f)

			assertEquals(exp_0, initial)

			Dim baos As New MemoryStream()
			Nd4j.write(baos, compressed)

			Dim serialized As INDArray = Nd4j.read(New MemoryStream(baos.toByteArray()))

			Dim decompressed_copy As INDArray = Nd4j.create(DataType.DOUBLE, initial.length())
			Nd4j.Executioner.thresholdDecode(serialized, decompressed_copy)

			assertEquals(exp_1, decompressed_copy)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBitmapEncoding1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBitmapEncoding1(ByVal backend As Nd4jBackend)
			Dim initial As INDArray = Nd4j.create(New Single() {0.0f, 0.0f, 1e-3f, -1e-3f, 0.0f, 0.0f})
			Dim exp_0 As INDArray = Nd4j.create(DataType.FLOAT, 6)
			Dim exp_1 As INDArray = initial.dup()

			Dim enc As INDArray = Nd4j.Executioner.bitmapEncode(initial, 1e-3)

			log.info("Encoded: {}", Arrays.toString(enc.data().asInt()))

			assertEquals(exp_0, initial)
			assertEquals(5, enc.data().length())

			log.info("Encoded: {}", Arrays.toString(enc.data().asInt()))

			Dim target As INDArray = Nd4j.create(DataType.FLOAT, 6)
			Nd4j.Executioner.bitmapDecode(enc, target)

			log.info("Target: {}", Arrays.toString(target.data().asFloat()))
			assertEquals(exp_1, target)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBitmapEncoding1_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBitmapEncoding1_1(ByVal backend As Nd4jBackend)
			Dim initial As INDArray = Nd4j.create(15)
			Dim exp_0 As INDArray = Nd4j.create(6)
			Dim exp_1 As INDArray = initial.dup()

			Dim enc As INDArray = Nd4j.Executioner.bitmapEncode(initial, 1e-3)

			'assertEquals(exp_0, initial);
			assertEquals(5, enc.data().length())

			initial = Nd4j.create(31)

			enc = Nd4j.Executioner.bitmapEncode(initial, 1e-3)

			assertEquals(6, enc.data().length())

			initial = Nd4j.create(32)

			enc = Nd4j.Executioner.bitmapEncode(initial, 1e-3)

			assertEquals(7, enc.data().length())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Tag(org.nd4j.common.tests.tags.TagNames.LONG_TEST) @Tag(org.nd4j.common.tests.tags.TagNames.LARGE_RESOURCES) public void testBitmapEncoding2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBitmapEncoding2(ByVal backend As Nd4jBackend)
			Dim initial As INDArray = Nd4j.create(DataType.FLOAT,40000000)
			Dim target As INDArray = Nd4j.create(DataType.FLOAT,initial.length())

			initial.addi(1e-3)

			Dim time1 As Long = DateTimeHelper.CurrentUnixTimeMillis()
			Dim enc As INDArray = Nd4j.Executioner.bitmapEncode(initial, 1e-3)
			Dim time2 As Long = DateTimeHelper.CurrentUnixTimeMillis()


			Nd4j.Executioner.bitmapDecode(enc, target)
			Dim time3 As Long = DateTimeHelper.CurrentUnixTimeMillis()

			log.info("Encode time: {}", time2 - time1)
			log.info("Decode time: {}", time3 - time2)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBitmapEncoding3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBitmapEncoding3(ByVal backend As Nd4jBackend)
			Nd4j.setDefaultDataTypes(DataType.FLOAT, DataType.FLOAT)
			Dim initial As INDArray = Nd4j.create(New Single() {0.0f, -6e-4f, 1e-3f, -1e-3f, 0.0f, 0.0f})
			Dim exp_0 As INDArray = Nd4j.create(New Single() {0.0f, -1e-4f, 0.0f, 0.0f, 0.0f, 0.0f})
			Dim exp_1 As INDArray = Nd4j.create(New Single() {0.0f, -5e-4f, 1e-3f, -1e-3f, 0.0f, 0.0f})


			Dim enc As INDArray = Nd4j.create(DataType.INT32, initial.length() \ 16 + 5)

			Dim elements As Long = Nd4j.Executioner.bitmapEncode(initial, enc, 1e-3)
			log.info("Encoded: {}", Arrays.toString(enc.data().asInt()))
			assertArrayEquals(New Integer() {6, 6, 981668463, 1, 655372}, enc.data().asInt())

			assertEquals(3, elements)

			assertEquals(exp_0, initial)

			Dim target As INDArray = Nd4j.create(6)

			Nd4j.Executioner.bitmapDecode(enc, target)
			log.info("Target: {}", Arrays.toString(target.data().asFloat()))
			assertEquals(exp_1, target.castTo(exp_1.dataType()))
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBitmapEncoding4(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBitmapEncoding4(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(119)
			Dim initial As INDArray = Nd4j.rand(New Integer(){1, 10000}, 0, 1, Nd4j.Random)
			Dim exp_1 As INDArray = initial.dup()

			Dim enc As INDArray = Nd4j.Executioner.bitmapEncode(initial, 1e-1)

			Nd4j.Executioner.bitmapDecode(enc, initial)

			assertEquals(exp_1, initial)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBitmapEncoding5(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBitmapEncoding5(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(119)
			Dim initial As INDArray = Nd4j.rand(New Integer(){10000}, -1, -0.5, Nd4j.Random)
			Dim exp_0 As INDArray = initial.dup().addi(1e-1)
			Dim exp_1 As INDArray = initial.dup()

			Dim enc As INDArray = Nd4j.Executioner.bitmapEncode(initial, 1e-1)
			assertEquals(exp_0, initial)

			Nd4j.Executioner.bitmapDecode(enc, initial)

			assertEquals(exp_1, initial)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBitmapEncoding6(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBitmapEncoding6(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(119)
			Dim initial As INDArray = Nd4j.rand(New Integer(){10000}, -1, 1, Nd4j.Random)
			Dim exp_1 As INDArray = initial.dup()

			Dim enc As INDArray = Nd4j.Executioner.bitmapEncode(initial, 1e-3)
			'assertEquals(exp_0, initial);

			Nd4j.Executioner.bitmapDecode(enc, initial)

			Dim f0 As val = exp_1.toFloatVector()
			Dim f1 As val = initial.toFloatVector()

			assertArrayEquals(f0, f1, 1e-5f)

			assertEquals(exp_1, initial)
		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace