Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic
Imports val = lombok.val
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports AllocUtil = org.nd4j.linalg.api.buffer.util.AllocUtil
Imports DataTypeUtil = org.nd4j.linalg.api.buffer.util.DataTypeUtil
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports Pooling2D = org.nd4j.linalg.api.ops.impl.layers.convolution.Pooling2D
Imports NDArrayCreationUtil = org.nd4j.linalg.checkutil.NDArrayCreationUtil
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports INDArrayIndex = org.nd4j.linalg.indexing.INDArrayIndex
Imports org.nd4j.common.primitives
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil
import static org.junit.jupiter.api.Assertions.assertArrayEquals
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.nd4j.linalg.indexing.NDArrayIndex.all
import static org.nd4j.linalg.indexing.NDArrayIndex.point

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

Namespace org.nd4j.linalg.convolution


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag public class ConvolutionTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class ConvolutionTests
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testIm2ColKnownValues(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testIm2ColKnownValues(ByVal backend As Nd4jBackend)
			'Input: w=3, h=3, depth=2, minibatch = 2
			'kH=2, kW=2
	'        
	'        ----- Input images -----
	'        example 0:
	'        depth 0     depth 1
	'        [ 0  1  2      [ 9 10 11
	'          3  4  5       12 13 14
	'          6  7  8]      15 16 17]
	'        example 1:
	'        [18 19 20      [27 28 29
	'         21 22 23       30 31 32
	'         24 25 26]      33 34 35]
	'        
	'         ----- Expected Output -----
	'         Shape: [miniBatch,depth,kH,kW,outH,outW]
	'         - example 0 -
	'         depth 0                        depth 1
	'         h0,w0      h0,w1               h0,w0      h0,w1
	'           0  1     1  2                 9 10      10 11
	'           3  4     4  5                12 13      13 14
	'        
	'         h1,w0      h1,w1               h1,w0      h1,w1
	'           3  4     4  5                12 13      13 14
	'           6  7     7  8                15 16      16 17
	'        
	'         - example 1 -
	'         depth 0                        depth 1
	'         h0,w0      h0,w1               h0,w0      h0,w1
	'          18 19     19 20               27 28      28 29
	'          21 22     22 23               30 31      31 32
	'        
	'         h1,w0      h1,w1               h1,w0      h1,w1
	'          21 22     22 23               30 31      31 32
	'          24 25     25 26               33 34      34 35
	'         

			Dim miniBatch As Integer = 2
			Dim depth As Integer = 2
			Dim height As Integer = 3
			Dim width As Integer = 3

			Dim outH As Integer = 2
			Dim outW As Integer = 2
			Dim kH As Integer = 2
			Dim kW As Integer = 2
			Dim sX As Integer = 1
			Dim sY As Integer = 1
			Dim pX As Integer = 0
			Dim pY As Integer = 0

			'Input data: shape [miniBatch,depth,height,width]
			Dim input As INDArray = Nd4j.create(New Integer() {miniBatch, depth, height, width}, "c"c)
			input.put(New INDArrayIndex() {point(0), point(0), all(), all()},
			Nd4j.create(New Double()() {
				New Double() {0, 1, 2},
				New Double() {3, 4, 5},
				New Double() {6, 7, 8}
			}))
			input.put(New INDArrayIndex() {point(0), point(1), all(), all()},
			Nd4j.create(New Double()() {
				New Double() {9, 10, 11},
				New Double() {12, 13, 14},
				New Double() {15, 16, 17}
			}))
			input.put(New INDArrayIndex() {point(1), point(0), all(), all()},
			Nd4j.create(New Double()() {
				New Double() {18, 19, 20},
				New Double() {21, 22, 23},
				New Double() {24, 25, 26}
			}))
			input.put(New INDArrayIndex() {point(1), point(1), all(), all()},
			Nd4j.create(New Double()() {
				New Double() {27, 28, 29},
				New Double() {30, 31, 32},
				New Double() {33, 34, 35}
			}))

			'Expected data:
			Dim expected As INDArray = Nd4j.create(New Integer() {miniBatch, depth, kH, kW, outH, outW}, "c"c)

			'Example 0
			'depth 0
			expected.put(New INDArrayIndex() {point(0), point(0), all(), all(), point(0), point(0)},
			Nd4j.create(New Double()() {
				New Double() {0, 1},
				New Double() {3, 4}
			}))
			expected.put(New INDArrayIndex() {point(0), point(0), all(), all(), point(0), point(1)},
			Nd4j.create(New Double()() {
				New Double() {1, 2},
				New Double() {4, 5}
			}))
			expected.put(New INDArrayIndex() {point(0), point(0), all(), all(), point(1), point(0)},
			Nd4j.create(New Double()() {
				New Double() {3, 4},
				New Double() {6, 7}
			}))
			expected.put(New INDArrayIndex() {point(0), point(0), all(), all(), point(1), point(1)},
			Nd4j.create(New Double()() {
				New Double() {4, 5},
				New Double() {7, 8}
			}))
			'depth 1
			expected.put(New INDArrayIndex() {point(0), point(1), all(), all(), point(0), point(0)},
			Nd4j.create(New Double()() {
				New Double() {9, 10},
				New Double() {12, 13}
			}))
			expected.put(New INDArrayIndex() {point(0), point(1), all(), all(), point(0), point(1)},
			Nd4j.create(New Double()() {
				New Double() {10, 11},
				New Double() {13, 14}
			}))
			expected.put(New INDArrayIndex() {point(0), point(1), all(), all(), point(1), point(0)},
			Nd4j.create(New Double()() {
				New Double() {12, 13},
				New Double() {15, 16}
			}))
			expected.put(New INDArrayIndex() {point(0), point(1), all(), all(), point(1), point(1)},
			Nd4j.create(New Double()() {
				New Double() {13, 14},
				New Double() {16, 17}
			}))

			'Example 1
			'depth 0
			expected.put(New INDArrayIndex() {point(1), point(0), all(), all(), point(0), point(0)},
			Nd4j.create(New Double()() {
				New Double() {18, 19},
				New Double() {21, 22}
			}))
			expected.put(New INDArrayIndex() {point(1), point(0), all(), all(), point(0), point(1)},
			Nd4j.create(New Double()() {
				New Double() {19, 20},
				New Double() {22, 23}
			}))
			expected.put(New INDArrayIndex() {point(1), point(0), all(), all(), point(1), point(0)},
			Nd4j.create(New Double()() {
				New Double() {21, 22},
				New Double() {24, 25}
			}))
			expected.put(New INDArrayIndex() {point(1), point(0), all(), all(), point(1), point(1)},
			Nd4j.create(New Double()() {
				New Double() {22, 23},
				New Double() {25, 26}
			}))
			'depth 1
			expected.put(New INDArrayIndex() {point(1), point(1), all(), all(), point(0), point(0)},
			Nd4j.create(New Double()() {
				New Double() {27, 28},
				New Double() {30, 31}
			}))
			expected.put(New INDArrayIndex() {point(1), point(1), all(), all(), point(0), point(1)},
			Nd4j.create(New Double()() {
				New Double() {28, 29},
				New Double() {31, 32}
			}))
			expected.put(New INDArrayIndex() {point(1), point(1), all(), all(), point(1), point(0)},
			Nd4j.create(New Double()() {
				New Double() {30, 31},
				New Double() {33, 34}
			}))
			expected.put(New INDArrayIndex() {point(1), point(1), all(), all(), point(1), point(1)},
			Nd4j.create(New Double()() {
				New Double() {31, 32},
				New Double() {34, 35}
			}))

			Dim [out] As INDArray = Convolution.im2col(input, kH, kW, sY, sX, pY, pX, False)
			assertEquals(expected, [out])

			'Now: test with a provided results array, where the results array has weird strides
			Dim out2 As INDArray = Nd4j.create(New Integer() {miniBatch, depth, outH, outW, kH, kW}, "c"c)
			Dim out2p As INDArray = out2.permute(0, 1, 4, 5, 2, 3)
			Convolution.im2col(input, kH, kW, sY, sX, pY, pX, False, out2p)
			assertEquals(expected, out2p)

			Dim out3 As INDArray = Nd4j.create(New Integer() {miniBatch, outH, outW, depth, kH, kW}, "c"c)
			Dim out3p As INDArray = out3.permute(0, 3, 4, 5, 1, 2)
			Convolution.im2col(input, kH, kW, sY, sX, pY, pX, False, out3p)
			assertEquals(expected, out3p)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testIm2ColKnownValuesDilated(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testIm2ColKnownValuesDilated(ByVal backend As Nd4jBackend)
			'Input: w=4, h=4, depth=1, minibatch = 2, dilation=2, stride 1
			'kH=2, kW=2
	'        
	'        ----- Input images -----
	'        example 0:
	'        depth 0
	'        [ 0  1  2  3
	'          4  5  6  7
	'          8  9 10 11
	'         12 13 14 15 ]
	'
	'        example 1:
	'        [16 17 18 19
	'         20 21 22 23
	'         24 25 26 27
	'         28 29 30 31 ]
	'
	'         ----- Expected Output -----
	'         Shape: [miniBatch,depth,kH,kW,outH,outW]
	'         - example 0 -
	'         depth 0
	'         h0,w0      h0,w1
	'           0  2     1  3
	'           8 10     9 11
	'
	'         h1,w0      h1,w1
	'           4  6     5  7
	'          12 14    13 15
	'
	'         - example 1 -
	'         depth 0
	'         h0,w0      h0,w1
	'          16 18     17 19
	'          24 26     25 27
	'
	'         h1,w0      h1,w1
	'          20 22     21 23
	'          28 30     29 31
	'         

			Dim miniBatch As Integer = 2
			Dim depth As Integer = 1
			Dim height As Integer = 4
			Dim width As Integer = 4

			Dim outH As Integer = 2
			Dim outW As Integer = 2
			Dim kH As Integer = 2
			Dim kW As Integer = 2
			Dim sX As Integer = 1
			Dim sY As Integer = 1
			Dim pX As Integer = 0
			Dim pY As Integer = 0
			Dim dh As Integer = 2
			Dim dw As Integer = 2

			'Input data: shape [miniBatch,depth,height,width]
			Dim input As INDArray = Nd4j.create(New Integer() {miniBatch, depth, height, width}, "c"c)
			input.put(New INDArrayIndex() {point(0), point(0), all(), all()},
			Nd4j.create(New Double()() {
				New Double() {0, 1, 2, 3},
				New Double() {4, 5, 6, 7},
				New Double() {8, 9, 10, 11},
				New Double() {12, 13, 14, 15}
			}))
			input.put(New INDArrayIndex() {point(1), point(0), all(), all()},
			Nd4j.create(New Double()() {
				New Double() {16, 17, 18, 19},
				New Double() {20, 21, 22, 23},
				New Double() {24, 25, 26, 27},
				New Double() {28, 29, 30, 31}
			}))

			'Expected data:
			Dim expected As INDArray = Nd4j.create(New Integer() {miniBatch, depth, kH, kW, outH, outW}, "c"c)

			'Example 0
			'depth 0
			expected.put(New INDArrayIndex() {point(0), point(0), all(), all(), point(0), point(0)},
			Nd4j.create(New Double()() {
				New Double() {0, 2},
				New Double() {8, 10}
			}))
			expected.put(New INDArrayIndex() {point(0), point(0), all(), all(), point(0), point(1)},
			Nd4j.create(New Double()() {
				New Double() {1, 3},
				New Double() {9, 11}
			}))
			expected.put(New INDArrayIndex() {point(0), point(0), all(), all(), point(1), point(0)},
			Nd4j.create(New Double()() {
				New Double() {4, 6},
				New Double() {12, 14}
			}))
			expected.put(New INDArrayIndex() {point(0), point(0), all(), all(), point(1), point(1)},
			Nd4j.create(New Double()() {
				New Double() {5, 7},
				New Double() {13, 15}
			}))

			'Example 1
			'depth 0
			expected.put(New INDArrayIndex() {point(1), point(0), all(), all(), point(0), point(0)},
			Nd4j.create(New Double()() {
				New Double() {16, 18},
				New Double() {24, 26}
			}))
			expected.put(New INDArrayIndex() {point(1), point(0), all(), all(), point(0), point(1)},
			Nd4j.create(New Double()() {
				New Double() {17, 19},
				New Double() {25, 27}
			}))
			expected.put(New INDArrayIndex() {point(1), point(0), all(), all(), point(1), point(0)},
			Nd4j.create(New Double()() {
				New Double() {20, 22},
				New Double() {28, 30}
			}))
			expected.put(New INDArrayIndex() {point(1), point(0), all(), all(), point(1), point(1)},
			Nd4j.create(New Double()() {
				New Double() {21, 23},
				New Double() {29, 31}
			}))

			Dim [out] As INDArray = Convolution.im2col(input, kH, kW, sY, sX, pY, pX, dh, dw, False)
			assertEquals(expected, [out])

			'Now: test with a provided results array, where the results array has weird strides
			Dim out2 As INDArray = Nd4j.create(New Integer() {miniBatch, depth, outH, outW, kH, kW}, "c"c)
			Dim out2p As INDArray = out2.permute(0, 1, 4, 5, 2, 3)
			Convolution.im2col(input, kH, kW, sY, sX, pY, pX, dh, dw, False, out2p)
			assertEquals(expected, out2p)

			Dim out3 As INDArray = Nd4j.create(New Integer() {miniBatch, outH, outW, depth, kH, kW}, "c"c)
			Dim out3p As INDArray = out3.permute(0, 3, 4, 5, 1, 2)
			Convolution.im2col(input, kH, kW, sY, sX, pY, pX, dh, dw, False, out3p)
			assertEquals(expected, out3p)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testIm2ColKnownValuesDilatedStrided(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testIm2ColKnownValuesDilatedStrided(ByVal backend As Nd4jBackend)
			'Input: w=5, h=5, depth=1, minibatch = 1, dilation=2, stride 2
			'kH=2, kW=2
	'        
	'        ----- Input images -----
	'        example 0:
	'        depth 0
	'        [ 0  1  2  3  4
	'          5  6  7  8  9
	'         10 11 12 13 14
	'         15 16 17 18 19
	'         20 21 22 23 24 ]
	'
	'         ----- Expected Output -----
	'         Shape: [miniBatch,depth,kH,kW,outH,outW]
	'         - example 0 -
	'         depth 0
	'         h0,w0      h0,w1
	'           0  2     2  4
	'          10 12    12 14
	'
	'         h1,w0      h1,w1
	'          10 12    12 14
	'          20 22    22 24
	'         

			Dim miniBatch As Integer = 1
			Dim depth As Integer = 1
			Dim height As Integer = 5
			Dim width As Integer = 5

			Dim outH As Integer = 2
			Dim outW As Integer = 2
			Dim kH As Integer = 2
			Dim kW As Integer = 2
			Dim sX As Integer = 2
			Dim sY As Integer = 2
			Dim pX As Integer = 0
			Dim pY As Integer = 0
			Dim dh As Integer = 2
			Dim dw As Integer = 2

			'Input data: shape [miniBatch,depth,height,width]
			Dim input As INDArray = Nd4j.create(New Integer() {miniBatch, depth, height, width}, "c"c)
			input.put(New INDArrayIndex() {point(0), point(0), all(), all()},
			Nd4j.create(New Double()() {
				New Double() {0, 1, 2, 3, 4},
				New Double() {5, 6, 7, 8, 9},
				New Double() {10, 11, 12, 13, 14},
				New Double() {15, 16, 17, 18, 19},
				New Double() {20, 21, 22, 23, 24}
			}))

			'Expected data:
			Dim expected As INDArray = Nd4j.create(New Integer() {miniBatch, depth, kH, kW, outH, outW}, "c"c)

			'Example 0
			'depth 0
			expected.put(New INDArrayIndex() {point(0), point(0), all(), all(), point(0), point(0)},
			Nd4j.create(New Double()() {
				New Double() {0, 2},
				New Double() {10, 12}
			}))
			expected.put(New INDArrayIndex() {point(0), point(0), all(), all(), point(0), point(1)},
			Nd4j.create(New Double()() {
				New Double() {2, 4},
				New Double() {12, 14}
			}))
			expected.put(New INDArrayIndex() {point(0), point(0), all(), all(), point(1), point(0)},
			Nd4j.create(New Double()() {
				New Double() {10, 12},
				New Double() {20, 22}
			}))
			expected.put(New INDArrayIndex() {point(0), point(0), all(), all(), point(1), point(1)},
			Nd4j.create(New Double()() {
				New Double() {12, 14},
				New Double() {22, 24}
			}))

			Dim [out] As INDArray = Convolution.im2col(input, kH, kW, sY, sX, pY, pX, dh, dw, False)
			assertEquals(expected, [out])

			'Now: test with a provided results array, where the results array has weird strides
			Dim out2 As INDArray = Nd4j.create(New Integer() {miniBatch, depth, outH, outW, kH, kW}, "c"c)
			Dim out2p As INDArray = out2.permute(0, 1, 4, 5, 2, 3)
			Convolution.im2col(input, kH, kW, sY, sX, pY, pX, dh, dw, False, out2p)
			assertEquals(expected, out2p)

			Dim out3 As INDArray = Nd4j.create(New Integer() {miniBatch, outH, outW, depth, kH, kW}, "c"c)
			Dim out3p As INDArray = out3.permute(0, 3, 4, 5, 1, 2)
			Convolution.im2col(input, kH, kW, sY, sX, pY, pX, dh, dw, False, out3p)
			assertEquals(expected, out3p)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testIm2ColKnownValuesMiniBatch3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testIm2ColKnownValuesMiniBatch3(ByVal backend As Nd4jBackend)
			'Input: w=3, h=3, depth=2, minibatch = 3
			'kH=2, kW=2
	'        
	'        ----- Input images -----
	'        example 0:
	'        depth 0     depth 1
	'        [ 0  1  2      [ 9 10 11
	'          3  4  5       12 13 14
	'          6  7  8]      15 16 17]
	'        example 1:
	'        [18 19 20      [27 28 29
	'         21 22 23       30 31 32
	'         24 25 26]      33 34 35]
	'        example 2:
	'        [36 37 38      [45 46 47
	'         39 40 41       48 49 50
	'         42 43 44]      51 52 53]
	'        
	'        
	'         ----- Expected Output -----
	'         Shape: [miniBatch,depth,kH,kW,outH,outW]
	'         - example 0 -
	'         depth 0                        depth 1
	'         h0,w0      h0,w1               h0,w0      h0,w1
	'           0  1     1  2                 9 10      10 11
	'           3  4     4  5                12 13      13 14
	'        
	'         h1,w0      h1,w1               h1,w0      h1,w1
	'           3  4     4  5                12 13      13 14
	'           6  7     7  8                15 16      16 17
	'        
	'         - example 1 -
	'         depth 0                        depth 1
	'         h0,w0      h0,w1               h0,w0      h0,w1
	'          18 19     19 20               27 28      28 29
	'          21 22     22 23               30 31      31 32
	'        
	'         h1,w0      h1,w1               h1,w0      h1,w1
	'          21 22     22 23               30 31      31 32
	'          24 25     25 26               33 34      34 35
	'        
	'         - example 2 -
	'         depth 0                        depth 1
	'         h0,w0      h0,w1               h0,w0      h0,w1
	'          36 37     37 38               45 46      46 47
	'          39 40     40 41               48 49      49 50
	'        
	'         h1,w0      h1,w1               h1,w0      h1,w1
	'          39 40     40 41               48 49      49 50
	'          42 43     43 44               51 52      52 53
	'         

			Dim miniBatch As Integer = 3
			Dim depth As Integer = 2
			Dim height As Integer = 3
			Dim width As Integer = 3

			Dim outH As Integer = 2
			Dim outW As Integer = 2
			Dim kH As Integer = 2
			Dim kW As Integer = 2
			Dim sX As Integer = 1
			Dim sY As Integer = 1
			Dim pX As Integer = 0
			Dim pY As Integer = 0

			'Input data: shape [miniBatch,depth,height,width]
			Dim input As INDArray = Nd4j.create(New Integer() {miniBatch, depth, height, width}, "c"c)
			input.put(New INDArrayIndex() {point(0), point(0), all(), all()},
			Nd4j.create(New Double()() {
				New Double() {0, 1, 2},
				New Double() {3, 4, 5},
				New Double() {6, 7, 8}
			}))
			input.put(New INDArrayIndex() {point(0), point(1), all(), all()},
			Nd4j.create(New Double()() {
				New Double() {9, 10, 11},
				New Double() {12, 13, 14},
				New Double() {15, 16, 17}
			}))
			input.put(New INDArrayIndex() {point(1), point(0), all(), all()},
			Nd4j.create(New Double()() {
				New Double() {18, 19, 20},
				New Double() {21, 22, 23},
				New Double() {24, 25, 26}
			}))
			input.put(New INDArrayIndex() {point(1), point(1), all(), all()},
			Nd4j.create(New Double()() {
				New Double() {27, 28, 29},
				New Double() {30, 31, 32},
				New Double() {33, 34, 35}
			}))
			input.put(New INDArrayIndex() {point(2), point(0), all(), all()},
			Nd4j.create(New Double()() {
				New Double() {36, 37, 38},
				New Double() {39, 40, 41},
				New Double() {42, 43, 44}
			}))
			input.put(New INDArrayIndex() {point(2), point(1), all(), all()},
			Nd4j.create(New Double()() {
				New Double() {45, 46, 47},
				New Double() {48, 49, 50},
				New Double() {51, 52, 53}
			}))

			'Expected data:
			Dim expected As INDArray = Nd4j.create(New Integer() {miniBatch, depth, kH, kW, outH, outW}, "c"c)

			'Example 0
			'depth 0
			expected.put(New INDArrayIndex() {point(0), point(0), all(), all(), point(0), point(0)},
			Nd4j.create(New Double()() {
				New Double() {0, 1},
				New Double() {3, 4}
			}))
			expected.put(New INDArrayIndex() {point(0), point(0), all(), all(), point(0), point(1)},
			Nd4j.create(New Double()() {
				New Double() {1, 2},
				New Double() {4, 5}
			}))
			expected.put(New INDArrayIndex() {point(0), point(0), all(), all(), point(1), point(0)},
			Nd4j.create(New Double()() {
				New Double() {3, 4},
				New Double() {6, 7}
			}))
			expected.put(New INDArrayIndex() {point(0), point(0), all(), all(), point(1), point(1)},
			Nd4j.create(New Double()() {
				New Double() {4, 5},
				New Double() {7, 8}
			}))
			'depth 1
			expected.put(New INDArrayIndex() {point(0), point(1), all(), all(), point(0), point(0)},
			Nd4j.create(New Double()() {
				New Double() {9, 10},
				New Double() {12, 13}
			}))
			expected.put(New INDArrayIndex() {point(0), point(1), all(), all(), point(0), point(1)},
			Nd4j.create(New Double()() {
				New Double() {10, 11},
				New Double() {13, 14}
			}))
			expected.put(New INDArrayIndex() {point(0), point(1), all(), all(), point(1), point(0)},
			Nd4j.create(New Double()() {
				New Double() {12, 13},
				New Double() {15, 16}
			}))
			expected.put(New INDArrayIndex() {point(0), point(1), all(), all(), point(1), point(1)},
			Nd4j.create(New Double()() {
				New Double() {13, 14},
				New Double() {16, 17}
			}))

			'Example 1
			'depth 0
			expected.put(New INDArrayIndex() {point(1), point(0), all(), all(), point(0), point(0)},
			Nd4j.create(New Double()() {
				New Double() {18, 19},
				New Double() {21, 22}
			}))
			expected.put(New INDArrayIndex() {point(1), point(0), all(), all(), point(0), point(1)},
			Nd4j.create(New Double()() {
				New Double() {19, 20},
				New Double() {22, 23}
			}))
			expected.put(New INDArrayIndex() {point(1), point(0), all(), all(), point(1), point(0)},
			Nd4j.create(New Double()() {
				New Double() {21, 22},
				New Double() {24, 25}
			}))
			expected.put(New INDArrayIndex() {point(1), point(0), all(), all(), point(1), point(1)},
			Nd4j.create(New Double()() {
				New Double() {22, 23},
				New Double() {25, 26}
			}))
			'depth 1
			expected.put(New INDArrayIndex() {point(1), point(1), all(), all(), point(0), point(0)},
			Nd4j.create(New Double()() {
				New Double() {27, 28},
				New Double() {30, 31}
			}))
			expected.put(New INDArrayIndex() {point(1), point(1), all(), all(), point(0), point(1)},
			Nd4j.create(New Double()() {
				New Double() {28, 29},
				New Double() {31, 32}
			}))
			expected.put(New INDArrayIndex() {point(1), point(1), all(), all(), point(1), point(0)},
			Nd4j.create(New Double()() {
				New Double() {30, 31},
				New Double() {33, 34}
			}))
			expected.put(New INDArrayIndex() {point(1), point(1), all(), all(), point(1), point(1)},
			Nd4j.create(New Double()() {
				New Double() {31, 32},
				New Double() {34, 35}
			}))

			'Example 2
			'depth 0
			expected.put(New INDArrayIndex() {point(2), point(0), all(), all(), point(0), point(0)},
			Nd4j.create(New Double()() {
				New Double() {36, 37},
				New Double() {39, 40}
			}))
			expected.put(New INDArrayIndex() {point(2), point(0), all(), all(), point(0), point(1)},
			Nd4j.create(New Double()() {
				New Double() {37, 38},
				New Double() {40, 41}
			}))
			expected.put(New INDArrayIndex() {point(2), point(0), all(), all(), point(1), point(0)},
			Nd4j.create(New Double()() {
				New Double() {39, 40},
				New Double() {42, 43}
			}))
			expected.put(New INDArrayIndex() {point(2), point(0), all(), all(), point(1), point(1)},
			Nd4j.create(New Double()() {
				New Double() {40, 41},
				New Double() {43, 44}
			}))
			'depth 1
			expected.put(New INDArrayIndex() {point(2), point(1), all(), all(), point(0), point(0)},
			Nd4j.create(New Double()() {
				New Double() {45, 46},
				New Double() {48, 49}
			}))
			expected.put(New INDArrayIndex() {point(2), point(1), all(), all(), point(0), point(1)},
			Nd4j.create(New Double()() {
				New Double() {46, 47},
				New Double() {49, 50}
			}))
			expected.put(New INDArrayIndex() {point(2), point(1), all(), all(), point(1), point(0)},
			Nd4j.create(New Double()() {
				New Double() {48, 49},
				New Double() {51, 52}
			}))
			expected.put(New INDArrayIndex() {point(2), point(1), all(), all(), point(1), point(1)},
			Nd4j.create(New Double()() {
				New Double() {49, 50},
				New Double() {52, 53}
			}))

			Dim [out] As INDArray = Convolution.im2col(input, kH, kW, sY, sX, pY, pX, False)
			assertEquals(expected, [out])

			'Now: test with a provided results array, where the results array has weird strides
			Dim out2 As INDArray = Nd4j.create(New Integer() {miniBatch, depth, outH, outW, kH, kW}, "c"c)
			Dim out2p As INDArray = out2.permute(0, 1, 4, 5, 2, 3)
			Convolution.im2col(input, kH, kW, sY, sX, pY, pX, False, out2p)
			assertEquals(expected, out2p)

			Dim out3 As INDArray = Nd4j.create(New Integer() {miniBatch, outH, outW, depth, kH, kW}, "c"c)
			Dim out3p As INDArray = out3.permute(0, 3, 4, 5, 1, 2)
			Convolution.im2col(input, kH, kW, sY, sX, pY, pX, False, out3p)
			assertEquals(expected, out3p)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testIm2ColSamePadding(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testIm2ColSamePadding(ByVal backend As Nd4jBackend)
			'Input: w=3, h=3, depth=2, minibatch = 2, kH/kW = 2, stride=1

			'Idea with same padding:
			'outH = ceil(inH / strideH)
			'outW = ceil(inW / strideW)

			Dim miniBatch As Integer = 2
			Dim depth As Integer = 2
			Dim inH As Integer = 3
			Dim inW As Integer = 3
			Dim strideH As Integer = 1
			Dim strideW As Integer = 1

			Dim kH As Integer = 2
			Dim kW As Integer = 2

			Dim outH As Integer = CInt(Math.Truncate(Math.Ceiling(inH / (CDbl(strideH)))))
			Dim outW As Integer = CInt(Math.Truncate(Math.Ceiling(inW / (CDbl(strideW)))))

			assertEquals(outH, inH)
			assertEquals(outW, inW)

			Dim sumPadHeight As Integer = ((outH - 1) * strideH + kH - inH)
			Dim padTop As Integer = sumPadHeight \ 2
			Dim padBottom As Integer = sumPadHeight - padTop

			Dim sumPadWidth As Integer = ((outW - 1) * strideW + kW - inW)
			Dim padLeft As Integer = sumPadWidth \ 2
			Dim padRight As Integer = sumPadWidth - padLeft

			Console.WriteLine("Output size: " & outH & ", " & outW)
			Console.WriteLine("Pad top/bottom: " & padTop & vbTab & padBottom)
			Console.WriteLine("Pad left/right: " & padLeft & vbTab & padRight)


	'        
	'        ----- Input images -----
	'        example 0:
	'        depth 0     depth 1
	'        [ 0  1  2      [ 9 10 11
	'          3  4  5       12 13 14
	'          6  7  8]      15 16 17]
	'        example 1:
	'        [18 19 20      [27 28 29
	'         21 22 23       30 31 32
	'         24 25 26]      33 34 35]
	'        
	'         ----- Expected Output -----
	'         Shape: [miniBatch,depth,kH,kW,outH,outW]
	'         - example 0 -
	'         depth 0                        depth 1
	'          h0,w0    h0,w1    h0,w2        h0,w0    h0,w1    h0,w2
	'           0  1     1  2     2  0         9 10    10 11    11  0
	'           3  4     4  5     5  0        12 13    13 14    14  0
	'        
	'          h1,w0    h1,w1    h1,w2        h1,w0    h1,w1    h1,w2
	'           3  4     4  5     5  0        12 13    13 14    14  0
	'           6  7     7  8     8  0        15 16    16 17    17  0
	'        
	'          h2,w0    h2,w1    h2,w2        h2,w0    h2,w1    h2,w2
	'           6  7     7  8     8  0        15 16    16 17    17  0
	'           0  0     0  0     0  0         0  0     0  0     0  0
	'        
	'         - example 1 -
	'         depth 0                        depth 1
	'         h0,w0     h0,w1    h0,w2        h0,w0    h0,w1    h0,w2
	'          18 19    19 20    20  0        27 28    28 29    29  0
	'          21 22    22 23    23  0        30 31    31 32    32  0
	'        
	'         h1,w0     h1,w1    h1,w2        h1,w0    h1,w1    h1,w2
	'          21 22    22 23    23  0        30 31    31 32    32  0
	'          24 25    25 26    26  0        33 34    34 35    35  0
	'        
	'         h2,w0     h2,w1    h2,w2        h2,w0    h2,w1    h2,w2
	'          24 25    25 26    26  0        33 34    34 35    35  0
	'           0  0     0  0     0  0         0  0     0  0     0  0
	'         

			'Input data: shape [miniBatch,depth,height,width]
			Dim input As INDArray = Nd4j.create(New Integer() {miniBatch, depth, inH, inW}, "c"c)
			input.put(New INDArrayIndex() {point(0), point(0), all(), all()},
			Nd4j.create(New Double()() {
				New Double() {0, 1, 2},
				New Double() {3, 4, 5},
				New Double() {6, 7, 8}
			}))
			input.put(New INDArrayIndex() {point(0), point(1), all(), all()},
			Nd4j.create(New Double()() {
				New Double() {9, 10, 11},
				New Double() {12, 13, 14},
				New Double() {15, 16, 17}
			}))
			input.put(New INDArrayIndex() {point(1), point(0), all(), all()},
			Nd4j.create(New Double()() {
				New Double() {18, 19, 20},
				New Double() {21, 22, 23},
				New Double() {24, 25, 26}
			}))
			input.put(New INDArrayIndex() {point(1), point(1), all(), all()},
			Nd4j.create(New Double()() {
				New Double() {27, 28, 29},
				New Double() {30, 31, 32},
				New Double() {33, 34, 35}
			}))

			'Expected data:
			Dim expected As INDArray = Nd4j.create(New Integer() {miniBatch, depth, kH, kW, outH, outW}, "c"c)

			'Example 0
			'depth 0
			expected.put(New INDArrayIndex() {point(0), point(0), all(), all(), point(0), point(0)},
			Nd4j.create(New Double()() {
				New Double() {0, 1},
				New Double() {3, 4}
			}))
			expected.put(New INDArrayIndex() {point(0), point(0), all(), all(), point(0), point(1)},
			Nd4j.create(New Double()() {
				New Double() {1, 2},
				New Double() {4, 5}
			}))
			expected.put(New INDArrayIndex() {point(0), point(0), all(), all(), point(0), point(2)},
			Nd4j.create(New Double()() {
				New Double() {2, 0},
				New Double() {5, 0}
			}))
			expected.put(New INDArrayIndex() {point(0), point(0), all(), all(), point(1), point(0)},
			Nd4j.create(New Double()() {
				New Double() {3, 4},
				New Double() {6, 7}
			}))
			expected.put(New INDArrayIndex() {point(0), point(0), all(), all(), point(1), point(1)},
			Nd4j.create(New Double()() {
				New Double() {4, 5},
				New Double() {7, 8}
			}))
			expected.put(New INDArrayIndex() {point(0), point(0), all(), all(), point(1), point(2)},
			Nd4j.create(New Double()() {
				New Double() {5, 0},
				New Double() {8, 0}
			}))
			expected.put(New INDArrayIndex() {point(0), point(0), all(), all(), point(2), point(0)},
			Nd4j.create(New Double()() {
				New Double() {6, 7},
				New Double() {0, 0}
			}))
			expected.put(New INDArrayIndex() {point(0), point(0), all(), all(), point(2), point(1)},
			Nd4j.create(New Double()() {
				New Double() {7, 8},
				New Double() {0, 0}
			}))
			expected.put(New INDArrayIndex() {point(0), point(0), all(), all(), point(2), point(2)},
			Nd4j.create(New Double()() {
				New Double() {8, 0},
				New Double() {0, 0}
			}))
			'depth 1
			expected.put(New INDArrayIndex() {point(0), point(1), all(), all(), point(0), point(0)},
			Nd4j.create(New Double()() {
				New Double() {9, 10},
				New Double() {12, 13}
			}))
			expected.put(New INDArrayIndex() {point(0), point(1), all(), all(), point(0), point(1)},
			Nd4j.create(New Double()() {
				New Double() {10, 11},
				New Double() {13, 14}
			}))
			expected.put(New INDArrayIndex() {point(0), point(1), all(), all(), point(0), point(2)},
			Nd4j.create(New Double()() {
				New Double() {11, 0},
				New Double() {14, 0}
			}))
			expected.put(New INDArrayIndex() {point(0), point(1), all(), all(), point(1), point(0)},
			Nd4j.create(New Double()() {
				New Double() {12, 13},
				New Double() {15, 16}
			}))
			expected.put(New INDArrayIndex() {point(0), point(1), all(), all(), point(1), point(1)},
			Nd4j.create(New Double()() {
				New Double() {13, 14},
				New Double() {16, 17}
			}))
			expected.put(New INDArrayIndex() {point(0), point(1), all(), all(), point(1), point(2)},
			Nd4j.create(New Double()() {
				New Double() {14, 0},
				New Double() {17, 0}
			}))
			expected.put(New INDArrayIndex() {point(0), point(1), all(), all(), point(2), point(0)},
			Nd4j.create(New Double()() {
				New Double() {15, 16},
				New Double() {0, 0}
			}))
			expected.put(New INDArrayIndex() {point(0), point(1), all(), all(), point(2), point(1)},
			Nd4j.create(New Double()() {
				New Double() {16, 17},
				New Double() {0, 0}
			}))
			expected.put(New INDArrayIndex() {point(0), point(1), all(), all(), point(2), point(2)},
			Nd4j.create(New Double()() {
				New Double() {17, 0},
				New Double() {0, 0}
			}))

			'Example 1
			'depth 0
			expected.put(New INDArrayIndex() {point(1), point(0), all(), all(), point(0), point(0)},
			Nd4j.create(New Double()() {
				New Double() {18, 19},
				New Double() {21, 22}
			}))
			expected.put(New INDArrayIndex() {point(1), point(0), all(), all(), point(0), point(1)},
			Nd4j.create(New Double()() {
				New Double() {19, 20},
				New Double() {22, 23}
			}))
			expected.put(New INDArrayIndex() {point(1), point(0), all(), all(), point(0), point(2)},
			Nd4j.create(New Double()() {
				New Double() {20, 0},
				New Double() {23, 0}
			}))
			expected.put(New INDArrayIndex() {point(1), point(0), all(), all(), point(1), point(0)},
			Nd4j.create(New Double()() {
				New Double() {21, 22},
				New Double() {24, 25}
			}))
			expected.put(New INDArrayIndex() {point(1), point(0), all(), all(), point(1), point(1)},
			Nd4j.create(New Double()() {
				New Double() {22, 23},
				New Double() {25, 26}
			}))
			expected.put(New INDArrayIndex() {point(1), point(0), all(), all(), point(1), point(2)},
			Nd4j.create(New Double()() {
				New Double() {23, 0},
				New Double() {26, 0}
			}))
			expected.put(New INDArrayIndex() {point(1), point(0), all(), all(), point(2), point(0)},
			Nd4j.create(New Double()() {
				New Double() {24, 25},
				New Double() {0, 0}
			}))
			expected.put(New INDArrayIndex() {point(1), point(0), all(), all(), point(2), point(1)},
			Nd4j.create(New Double()() {
				New Double() {25, 26},
				New Double() {0, 0}
			}))
			expected.put(New INDArrayIndex() {point(1), point(0), all(), all(), point(2), point(2)},
			Nd4j.create(New Double()() {
				New Double() {26, 0},
				New Double() {0, 0}
			}))

			'depth 1
			expected.put(New INDArrayIndex() {point(1), point(1), all(), all(), point(0), point(0)},
			Nd4j.create(New Double()() {
				New Double() {27, 28},
				New Double() {30, 31}
			}))
			expected.put(New INDArrayIndex() {point(1), point(1), all(), all(), point(0), point(1)},
			Nd4j.create(New Double()() {
				New Double() {28, 29},
				New Double() {31, 32}
			}))
			expected.put(New INDArrayIndex() {point(1), point(1), all(), all(), point(0), point(2)},
			Nd4j.create(New Double()() {
				New Double() {29, 0},
				New Double() {32, 0}
			}))
			expected.put(New INDArrayIndex() {point(1), point(1), all(), all(), point(1), point(0)},
			Nd4j.create(New Double()() {
				New Double() {30, 31},
				New Double() {33, 34}
			}))
			expected.put(New INDArrayIndex() {point(1), point(1), all(), all(), point(1), point(1)},
			Nd4j.create(New Double()() {
				New Double() {31, 32},
				New Double() {34, 35}
			}))
			expected.put(New INDArrayIndex() {point(1), point(1), all(), all(), point(1), point(2)},
			Nd4j.create(New Double()() {
				New Double() {32, 0},
				New Double() {35, 0}
			}))
			expected.put(New INDArrayIndex() {point(1), point(1), all(), all(), point(2), point(0)},
			Nd4j.create(New Double()() {
				New Double() {33, 34},
				New Double() {0, 0}
			}))
			expected.put(New INDArrayIndex() {point(1), point(1), all(), all(), point(2), point(1)},
			Nd4j.create(New Double()() {
				New Double() {34, 35},
				New Double() {0, 0}
			}))
			expected.put(New INDArrayIndex() {point(1), point(1), all(), all(), point(2), point(2)},
			Nd4j.create(New Double()() {
				New Double() {35, 0},
				New Double() {0, 0}
			}))

			'[miniBatch,depth,kH,kW,outH,outW]
			Dim outAlloc As INDArray = Nd4j.create(miniBatch, depth, kH, kW, outH, outW)
			Dim [out] As INDArray = Convolution.im2col(input, kH, kW, strideH, strideW, padTop, padLeft, True, outAlloc)
			'        System.out.println("Output shape: " + Arrays.toString(out.shape()));
			'
			'        for( int mb = 0; mb<2; mb++ ){
			'            for( int d = 0; d<2; d++ ){
			'                for( int h=0; h<3; h++ ){
			'                    for( int w=0; w<3; w++ ){
			'                        INDArrayIndex[] indx = new INDArrayIndex[]{NDArrayIndex.point(mb),NDArrayIndex.point(d),NDArrayIndex.all(),NDArrayIndex.all(), NDArrayIndex.point(h), NDArrayIndex.point(w)};
			'                        INDArray e = expected.get(indx);
			'                        INDArray a = out.get(indx);
			'
			'                        System.out.println("minibatch = " + mb + ", depth = " + depth + ", outY = " + h + ", outX = " + w + "\t" + (e.equals(a) ? "ok" : "FAILED"));
			'                        System.out.println(e);
			'                        System.out.println(a);
			'                        System.out.println("\n-------------------------");
			'                    }
			'                }
			'
			'            }
			'        }


			assertEquals(expected, [out])

			'Now: test with a provided results array, where the results array has weird strides
			Dim out2 As INDArray = Nd4j.create(New Integer() {miniBatch, depth, outH, outW, kH, kW}, "c"c)
			Dim out2p As INDArray = out2.permute(0, 1, 4, 5, 2, 3)
			Convolution.im2col(input, kH, kW, strideH, strideW, padTop, padLeft, True, out2p)
			assertEquals(expected, out2p)

			Dim out3 As INDArray = Nd4j.create(New Integer() {miniBatch, outH, outW, depth, kH, kW}, "c"c)
			Dim out3p As INDArray = out3.permute(0, 3, 4, 5, 1, 2)
			Convolution.im2col(input, kH, kW, strideH, strideW, padTop, padLeft, True, out3p)
			assertEquals(expected, out3p)



			'/////////
			'Finally: Check col2im with the same shapes. This doesn't check the results, more 'does it crash or not'

			Dim col2imResult As INDArray = Nd4j.create(input.shape())
			Dim col2im As INDArray = Convolution.col2im([out], col2imResult, strideH, strideW, padTop, padLeft, inH, inW, 1, 1)
			Console.WriteLine(Arrays.toString(col2im.data().asDouble()))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testIm2ColSamePaddingStride2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testIm2ColSamePaddingStride2(ByVal backend As Nd4jBackend)
			'Input: h=3, w=4, depth=2, minibatch = 1, kH/kW = 3, stride=2

			'Idea with same padding:
			'outH = ceil(inH / strideH)
			'outW = ceil(inW / strideW)

			Dim miniBatch As Integer = 1
			Dim depth As Integer = 2
			Dim inH As Integer = 3
			Dim inW As Integer = 4
			Dim strideH As Integer = 2
			Dim strideW As Integer = 2

			Dim kH As Integer = 3
			Dim kW As Integer = 3

			Dim outH As Integer = CInt(Math.Truncate(Math.Ceiling(inH / (CDbl(strideH)))))
			Dim outW As Integer = CInt(Math.Truncate(Math.Ceiling(inW / (CDbl(strideW)))))

			assertEquals(2, outH) 'ceil(3/2) = 2
			assertEquals(2, outW) 'ceil(4/2) = 2

			Dim sumPadHeight As Integer = ((outH - 1) * strideH + kH - inH)
			Dim padTop As Integer = sumPadHeight \ 2
			Dim padBottom As Integer = sumPadHeight - padTop

			assertEquals(1, padTop)
			assertEquals(1, padBottom)

			Dim sumPadWidth As Integer = ((outW - 1) * strideW + kW - inW)
			Dim padLeft As Integer = sumPadWidth \ 2
			Dim padRight As Integer = sumPadWidth - padLeft

			assertEquals(0, padLeft)
			assertEquals(1, padRight)

			Console.WriteLine("Output size: " & outH & ", " & outW)
			Console.WriteLine("Pad top/bottom: " & padTop & vbTab & padBottom)
			Console.WriteLine("Pad left/right: " & padLeft & vbTab & padRight)


	'        
	'        ----- Input images -----
	'        example 0:
	'        depth 0       depth 1
	'        [ 0  1  2  3      [12 13 14 15
	'          4  5  6  7       16 17 18 19
	'          8  9 10 11]      20 21 22 23]
	'        
	'         ----- Expected Output -----
	'         Shape: [miniBatch,depth,kH,kW,outH,outW]
	'         - example 0 -
	'         depth 0                        depth 1
	'          h0,w0        h0,w1            h0,w0       h0,w1
	'           0  0  0     0  0  0           0  0  0    0  0  0
	'           0  1  2     2  3  0          12 13 14   14 15  0
	'           4  5  6     6  7  0          16 17 18   18 19  0
	'        
	'          h1,w0
	'           4  5  6     6  7  0          16 17 18   18 19  0
	'           8  9 10    10 11  0          20 21 22   22 23  0
	'           0  0  0     0  0  0           0  0  0    0  0  0
	'         

			'Input data: shape [miniBatch,depth,height,width]
			Dim input As INDArray = Nd4j.create(New Integer() {miniBatch, depth, inH, inW}, "c"c)
			input.put(New INDArrayIndex() {point(0), point(0), all(), all()},
			Nd4j.create(New Double()() {
				New Double() {0, 1, 2, 3},
				New Double() {4, 5, 6, 7},
				New Double() {8, 9, 10, 11}
			}))
			input.put(New INDArrayIndex() {point(0), point(1), all(), all()},
			Nd4j.create(New Double()() {
				New Double() {12, 13, 14, 15},
				New Double() {16, 17, 18, 19},
				New Double() {20, 21, 22, 23}
			}))

			'Expected data:
			Dim expected As INDArray = Nd4j.create(New Integer() {miniBatch, depth, kH, kW, outH, outW}, "c"c)

			'Example 0
			'depth 0
			expected.put(New INDArrayIndex() {point(0), point(0), all(), all(), point(0), point(0)},
			Nd4j.create(New Double()() {
				New Double() {0, 0, 0},
				New Double() {0, 1, 2},
				New Double() {4, 5, 6}
			}))
			expected.put(New INDArrayIndex() {point(0), point(0), all(), all(), point(0), point(1)},
			Nd4j.create(New Double()() {
				New Double() {0, 0, 0},
				New Double() {2, 3, 0},
				New Double() {6, 7, 0}
			}))
			expected.put(New INDArrayIndex() {point(0), point(0), all(), all(), point(1), point(0)},
			Nd4j.create(New Double()() {
				New Double() {4, 5, 6},
				New Double() {8, 9, 10},
				New Double() {0, 0, 0}
			}))
			expected.put(New INDArrayIndex() {point(0), point(0), all(), all(), point(1), point(1)},
			Nd4j.create(New Double()() {
				New Double() {6, 7, 0},
				New Double() {10, 11, 0},
				New Double() {0, 0, 0}
			}))
			'depth 1
			expected.put(New INDArrayIndex() {point(0), point(1), all(), all(), point(0), point(0)},
			Nd4j.create(New Double()() {
				New Double() {0, 0, 0},
				New Double() {12, 13, 14},
				New Double() {16, 17, 18}
			}))
			expected.put(New INDArrayIndex() {point(0), point(1), all(), all(), point(0), point(1)},
			Nd4j.create(New Double()() {
				New Double() {0, 0, 0},
				New Double() {14, 15, 0},
				New Double() {18, 19, 0}
			}))
			expected.put(New INDArrayIndex() {point(0), point(1), all(), all(), point(1), point(0)},
			Nd4j.create(New Double()() {
				New Double() {16, 17, 18},
				New Double() {20, 21, 22},
				New Double() {0, 0, 0}
			}))
			expected.put(New INDArrayIndex() {point(0), point(1), all(), all(), point(1), point(1)},
			Nd4j.create(New Double()() {
				New Double() {18, 19, 0},
				New Double() {22, 23, 0},
				New Double() {0, 0, 0}
			}))

			'[miniBatch,depth,kH,kW,outH,outW]
			Dim outAlloc As INDArray = Nd4j.create(miniBatch, depth, kH, kW, outH, outW)
			Dim [out] As INDArray = Convolution.im2col(input, kH, kW, strideH, strideW, padTop, padLeft, True, outAlloc)
			'        System.out.println("Output shape: " + Arrays.toString(out.shape()));
			'
			'        for( int mb = 0; mb<2; mb++ ){
			'            for( int d = 0; d<2; d++ ){
			'                for( int h=0; h<3; h++ ){
			'                    for( int w=0; w<3; w++ ){
			'                        INDArrayIndex[] indx = new INDArrayIndex[]{NDArrayIndex.point(mb),NDArrayIndex.point(d),NDArrayIndex.all(),NDArrayIndex.all(), NDArrayIndex.point(h), NDArrayIndex.point(w)};
			'                        INDArray e = expected.get(indx);
			'                        INDArray a = out.get(indx);
			'
			'                        System.out.println("minibatch = " + mb + ", depth = " + depth + ", outY = " + h + ", outX = " + w + "\t" + (e.equals(a) ? "ok" : "FAILED"));
			'                        System.out.println(e);
			'                        System.out.println(a);
			'                        System.out.println("\n-------------------------");
			'                    }
			'                }
			'
			'            }
			'        }


			assertEquals(expected, [out])

			'Now: test with a provided results array, where the results array has weird strides
			Dim out2 As INDArray = Nd4j.create(New Integer() {miniBatch, depth, outH, outW, kH, kW}, "c"c)
			Dim out2p As INDArray = out2.permute(0, 1, 4, 5, 2, 3)
			Convolution.im2col(input, kH, kW, strideH, strideW, padTop, padLeft, True, out2p)
			assertEquals(expected, out2p)

			Dim out3 As INDArray = Nd4j.create(New Integer() {miniBatch, outH, outW, depth, kH, kW}, "c"c)
			Dim out3p As INDArray = out3.permute(0, 3, 4, 5, 1, 2)
			Convolution.im2col(input, kH, kW, strideH, strideW, padTop, padLeft, True, out3p)
			assertEquals(expected, out3p)


			'/////////
			'Finally: Check col2im with the same shapes. This doesn't check the results, more 'does it crash or not'

			Dim col2imResult As INDArray = Nd4j.create(input.shape())
			Dim col2im As INDArray = Convolution.col2im([out], col2imResult, strideH, strideW, padTop, padLeft, inH, inW, 1, 1)
			Console.WriteLine(Arrays.toString(col2im.data().asDouble()))
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCol2ImSamePaddingStride2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCol2ImSamePaddingStride2(ByVal backend As Nd4jBackend)
			'Input: h=3, w=4, depth=2, minibatch = 1, kH/kW = 3, stride=2

			'Idea with same padding:
			'outH = ceil(inH / strideH)
			'outW = ceil(inW / strideW)

			Dim miniBatch As Integer = 1
			Dim depth As Integer = 2
			Dim inH As Integer = 3
			Dim inW As Integer = 4
			Dim strideH As Integer = 2
			Dim strideW As Integer = 2

			Dim kH As Integer = 3
			Dim kW As Integer = 3

			Dim outH As Integer = CInt(Math.Truncate(Math.Ceiling(inH / (CDbl(strideH)))))
			Dim outW As Integer = CInt(Math.Truncate(Math.Ceiling(inW / (CDbl(strideW)))))

			assertEquals(2, outH) 'ceil(3/2) = 2
			assertEquals(2, outW) 'ceil(4/2) = 2

			Dim sumPadHeight As Integer = ((outH - 1) * strideH + kH - inH)
			Dim padTop As Integer = sumPadHeight \ 2
			Dim padBottom As Integer = sumPadHeight - padTop

			assertEquals(1, padTop)
			assertEquals(1, padBottom)

			Dim sumPadWidth As Integer = ((outW - 1) * strideW + kW - inW)
			Dim padLeft As Integer = sumPadWidth \ 2
			Dim padRight As Integer = sumPadWidth - padLeft

			assertEquals(0, padLeft)
			assertEquals(1, padRight)

	'        System.out.println("Output size: " + outH + ", " + outW);
	'        System.out.println("Pad top/bottom: " + padTop + "\t" + padBottom);
	'        System.out.println("Pad left/right: " + padLeft + "\t" + padRight);


	'        
	'        ----- Input images -----
	'        example 0:
	'        depth 0       depth 1
	'        [ 0  1  2  3      [12 13 14 15
	'          4  5  6  7       16 17 18 19
	'          8  9 10 11]      20 21 22 23]
	'        
	'         ----- Expected Output -----
	'         Shape: [miniBatch,depth,kH,kW,outH,outW]
	'         - example 0 -
	'         depth 0                        depth 1
	'          h0,w0        h0,w1            h0,w0       h0,w1
	'           0  0  0     0  0  0           0  0  0    0  0  0
	'           0  1  2     2  3  0          12 13 14   14 15  0
	'           4  5  6     6  7  0          16 17 18   18 19  0
	'        
	'          h1,w0
	'           4  5  6     6  7  0          16 17 18   18 19  0
	'           8  9 10    10 11  0          20 21 22   22 23  0
	'           0  0  0     0  0  0           0  0  0    0  0  0
	'         

	'        
	'        Col2im result:
	'        
	'        example 0:
	'        depth 0           depth 1
	'        [ 0  1  4  3      [12 13 28 15
	'          8 10 24 14       32 34 72 38
	'          8  9 20 11]      20 21 44 23]
	'         

			'Input data: shape [miniBatch,depth,height,width]
			'        INDArray input = Nd4j.create(new int[]{miniBatch,depth,inH,inW},'c');
			'        input.put(new INDArrayIndex[]{NDArrayIndex.point(0), NDArrayIndex.point(0),NDArrayIndex.all(), NDArrayIndex.all()}, Nd4j.create(new double[][]{{0,1,2,3},{4,5,6,7},{8,9,10,11}}));
			'        input.put(new INDArrayIndex[]{NDArrayIndex.point(0), NDArrayIndex.point(1),NDArrayIndex.all(), NDArrayIndex.all()}, Nd4j.create(new double[][]{{12,13,14,15},{16,17,18,19},{20,21,22,23}}));

			Dim col6d As INDArray = Nd4j.create(New Integer() {miniBatch, depth, kH, kW, outH, outW}, "c"c)

			'Example 0
			'depth 0
			col6d.put(New INDArrayIndex() {point(0), point(0), all(), all(), point(0), point(0)},
			Nd4j.create(New Double()() {
				New Double() {0, 0, 0},
				New Double() {0, 1, 2},
				New Double() {4, 5, 6}
			}))
			col6d.put(New INDArrayIndex() {point(0), point(0), all(), all(), point(0), point(1)},
			Nd4j.create(New Double()() {
				New Double() {0, 0, 0},
				New Double() {2, 3, 0},
				New Double() {6, 7, 0}
			}))
			col6d.put(New INDArrayIndex() {point(0), point(0), all(), all(), point(1), point(0)},
			Nd4j.create(New Double()() {
				New Double() {4, 5, 6},
				New Double() {8, 9, 10},
				New Double() {0, 0, 0}
			}))
			col6d.put(New INDArrayIndex() {point(0), point(0), all(), all(), point(1), point(1)},
			Nd4j.create(New Double()() {
				New Double() {6, 7, 0},
				New Double() {10, 11, 0},
				New Double() {0, 0, 0}
			}))
			'depth 1
			col6d.put(New INDArrayIndex() {point(0), point(1), all(), all(), point(0), point(0)},
			Nd4j.create(New Double()() {
				New Double() {0, 0, 0},
				New Double() {12, 13, 14},
				New Double() {16, 17, 18}
			}))
			col6d.put(New INDArrayIndex() {point(0), point(1), all(), all(), point(0), point(1)},
			Nd4j.create(New Double()() {
				New Double() {0, 0, 0},
				New Double() {14, 15, 0},
				New Double() {18, 19, 0}
			}))
			col6d.put(New INDArrayIndex() {point(0), point(1), all(), all(), point(1), point(0)},
			Nd4j.create(New Double()() {
				New Double() {16, 17, 18},
				New Double() {20, 21, 22},
				New Double() {0, 0, 0}
			}))
			col6d.put(New INDArrayIndex() {point(0), point(1), all(), all(), point(1), point(1)},
			Nd4j.create(New Double()() {
				New Double() {18, 19, 0},
				New Double() {22, 23, 0},
				New Double() {0, 0, 0}
			}))


			'Expected result:
			Dim expected As INDArray = Nd4j.create(miniBatch, depth, inH, inW)
			expected.put(New INDArrayIndex() {point(0), point(0), all(), all()},
			Nd4j.create(New Double()() {
				New Double() {0, 1, 4, 3},
				New Double() {8, 10, 24, 14},
				New Double() {8, 9, 20, 11}
			}))
			expected.put(New INDArrayIndex() {point(0), point(1), all(), all()},
			Nd4j.create(New Double()() {
				New Double() {12, 13, 28, 15},
				New Double() {32, 34, 72, 38},
				New Double() {20, 21, 44, 23}
			}))


			Dim col2imResult As INDArray = Nd4j.create(miniBatch, depth, inH, inW)
			Dim col2im As INDArray = Convolution.col2im(col6d, col2imResult, strideH, strideW, padTop, padLeft, inH, inW, 1, 1)

			assertEquals(expected, col2im)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCol2ImSamePaddingStride1Dilation2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCol2ImSamePaddingStride1Dilation2(ByVal backend As Nd4jBackend)
			'Input: h=4, w=5, depth=1, minibatch = 1, kH/kW = 2, stride=1, dilation 2

			'Idea with same padding:
			'outH = ceil(inH / strideH)
			'outW = ceil(inW / strideW)

			Dim miniBatch As Integer = 1
			Dim depth As Integer = 1
			Dim inH As Integer = 4
			Dim inW As Integer = 5
			Dim strideH As Integer = 1
			Dim strideW As Integer = 1
			Dim dH As Integer = 2
			Dim dW As Integer = 2

			Dim kH As Integer = 2
			Dim kW As Integer = 2

			Dim effectiveKH As Integer = kH + (kH-1)*(dH-1)
			Dim effectiveKW As Integer = kW + (kW-1)*(dW-1)

			Dim outH As Integer = CInt(Math.Truncate(Math.Ceiling(inH / (CDbl(strideH)))))
			Dim outW As Integer = CInt(Math.Truncate(Math.Ceiling(inW / (CDbl(strideW)))))

			assertEquals(5, outW) 'ceil(5/1) = 5
			assertEquals(4, outH) 'ceil(4/1) = 5

			Dim sumPadHeight As Integer = ((outH - 1) * strideH + effectiveKH - inH)
			Dim padTop As Integer = sumPadHeight \ 2
			Dim padBottom As Integer = sumPadHeight - padTop

			assertEquals(1, padTop)
			assertEquals(1, padBottom)

			Dim sumPadWidth As Integer = ((outW - 1) * strideW + effectiveKW - inW)
			Dim padLeft As Integer = sumPadWidth \ 2
			Dim padRight As Integer = sumPadWidth - padLeft

			assertEquals(1, padLeft)
			assertEquals(1, padRight)

	'        System.out.println("Output size: " + outH + ", " + outW);
	'        System.out.println("Pad top/bottom: " + padTop + "\t" + padBottom);
	'        System.out.println("Pad left/right: " + padLeft + "\t" + padRight);


	'        
	'        ----- Input images -----
	'        example 0:
	'        depth 0
	'        [ 0  1  2  3  4
	'          5  6  7  8  9
	'         10 11 12 13 14
	'         15 16 17 18 19 ]
	'
	'         Effective input, with padding:
	'        [ 0  0  0  0  0  0  0
	'          0  0  1  2  3  4  0
	'          0  5  6  7  8  9  0
	'          0 10 11 12 13 14  0
	'          0 15 16 17 18 19  0
	'          0  0  0  0  0  0  0]
	'
	'         ----- Expected Output -----
	'         Shape: [miniBatch,depth,kH,kW,outH,outW]
	'         - example 0 -
	'         depth 0
	'          h0,w0     h0,w1    h0,w2    h0,w3    h0,w4
	'           0  0     0  0     0  0     0  0     0  0
	'           0  6     5  7     6  8     7  9     8  0
	'
	'          h0,w0     h0,w1    h0,w2    h0,w3    h0,w4
	'           0  1     0  2     1  3     2  4     3  0
	'           0 11    10 12    11 13    12 14    13  0
	'
	'          h0,w0     h0,w1    h0,w2    h0,w3    h0,w4
	'           0  6     5  7     6  8     7  9     8  0
	'           0 16    15 17    16 18    17 19    18  0
	'
	'          h0,w0     h0,w1    h0,w2    h0,w3    h0,w4
	'           0 11    10 12    11 13    12 14    13  0
	'           0  0     0  0     0  0     0  0     0  0
	'         

	'        
	'        Col2im result:
	'
	'        example 0:
	'        depth 0
	'        [ 0  2  4  6  4
	'         10 24 28 32 18
	'         20 44 48 52 28
	'         15 32 34 36 19]
	'         

			'Input data: shape [miniBatch,depth,height,width]
			Dim input As INDArray = Nd4j.create(New Integer(){miniBatch, depth, inH, inW},"c"c)
			input.put(New INDArrayIndex(){point(0), point(0), all(), all()},
			Nd4j.create(New Double()(){
				New Double() {0, 1, 2, 3, 4},
				New Double() {5, 6, 7, 8, 9},
				New Double() {10, 11, 12, 13, 14},
				New Double() {15, 16, 17, 18, 19}
			}))

			Dim col6d As INDArray = Nd4j.create(New Integer() {miniBatch, depth, kH, kW, outH, outW}, "c"c)

			'Example 0
			'depth 0
			'Iterate over width, then height
			col6d.put(New INDArrayIndex() {point(0), point(0), all(), all(), point(0), point(0)},
			Nd4j.create(New Double()() {
				New Double() {0, 0},
				New Double() {0, 6}
			}))
			col6d.put(New INDArrayIndex() {point(0), point(0), all(), all(), point(0), point(1)},
			Nd4j.create(New Double()() {
				New Double() {0, 0},
				New Double() {5, 7}
			}))
			col6d.put(New INDArrayIndex() {point(0), point(0), all(), all(), point(0), point(2)},
			Nd4j.create(New Double()() {
				New Double() {0, 0},
				New Double() {6, 8}
			}))
			col6d.put(New INDArrayIndex() {point(0), point(0), all(), all(), point(0), point(3)},
			Nd4j.create(New Double()() {
				New Double() {0, 0},
				New Double() {7, 9}
			}))
			col6d.put(New INDArrayIndex() {point(0), point(0), all(), all(), point(0), point(4)},
			Nd4j.create(New Double()() {
				New Double() {0, 0},
				New Double() {8, 0}
			}))

			col6d.put(New INDArrayIndex() {point(0), point(0), all(), all(), point(1), point(0)},
			Nd4j.create(New Double()() {
				New Double() {0, 1},
				New Double() {0, 11}
			}))
			col6d.put(New INDArrayIndex() {point(0), point(0), all(), all(), point(1), point(1)},
			Nd4j.create(New Double()() {
				New Double() {0, 2},
				New Double() {10, 12}
			}))
			col6d.put(New INDArrayIndex() {point(0), point(0), all(), all(), point(1), point(2)},
			Nd4j.create(New Double()() {
				New Double() {1, 3},
				New Double() {11, 13}
			}))
			col6d.put(New INDArrayIndex() {point(0), point(0), all(), all(), point(1), point(3)},
			Nd4j.create(New Double()() {
				New Double() {2, 4},
				New Double() {12, 14}
			}))
			col6d.put(New INDArrayIndex() {point(0), point(0), all(), all(), point(1), point(4)},
			Nd4j.create(New Double()() {
				New Double() {3, 0},
				New Double() {13, 0}
			}))

			col6d.put(New INDArrayIndex() {point(0), point(0), all(), all(), point(2), point(0)},
			Nd4j.create(New Double()() {
				New Double() {0, 6},
				New Double() {0, 16}
			}))
			col6d.put(New INDArrayIndex() {point(0), point(0), all(), all(), point(2), point(1)},
			Nd4j.create(New Double()() {
				New Double() {5, 7},
				New Double() {15, 17}
			}))
			col6d.put(New INDArrayIndex() {point(0), point(0), all(), all(), point(2), point(2)},
			Nd4j.create(New Double()() {
				New Double() {6, 8},
				New Double() {16, 18}
			}))
			col6d.put(New INDArrayIndex() {point(0), point(0), all(), all(), point(2), point(3)},
			Nd4j.create(New Double()() {
				New Double() {7, 9},
				New Double() {17, 19}
			}))
			col6d.put(New INDArrayIndex() {point(0), point(0), all(), all(), point(2), point(4)},
			Nd4j.create(New Double()() {
				New Double() {8, 0},
				New Double() {18, 0}
			}))

			col6d.put(New INDArrayIndex() {point(0), point(0), all(), all(), point(3), point(0)},
			Nd4j.create(New Double()() {
				New Double() {0, 11},
				New Double() {0, 0}
			}))
			col6d.put(New INDArrayIndex() {point(0), point(0), all(), all(), point(3), point(1)},
			Nd4j.create(New Double()() {
				New Double() {10, 12},
				New Double() {0, 0}
			}))
			col6d.put(New INDArrayIndex() {point(0), point(0), all(), all(), point(3), point(2)},
			Nd4j.create(New Double()() {
				New Double() {11, 13},
				New Double() {0, 0}
			}))
			col6d.put(New INDArrayIndex() {point(0), point(0), all(), all(), point(3), point(3)},
			Nd4j.create(New Double()() {
				New Double() {12, 14},
				New Double() {0, 0}
			}))
			col6d.put(New INDArrayIndex() {point(0), point(0), all(), all(), point(3), point(4)},
			Nd4j.create(New Double()() {
				New Double() {13, 0},
				New Double() {0, 0}
			}))



			'Check im2col:
			Dim im2col As INDArray = Convolution.im2col(input, kH, kW, strideH, strideW, padTop, padLeft, dH, dW, True)


			For j As Integer = 0 To outH - 1
				For i As Integer = 0 To outW - 1
					Dim exp As INDArray = col6d.get(point(0), point(0), all(), all(), point(j), point(i))
					Dim act As INDArray = im2col.get(point(0), point(0), all(), all(), point(j), point(i))
					If Not exp.Equals(act) Then
						Console.WriteLine(i & vbTab & j)
						Console.WriteLine(exp)
						Console.WriteLine()
						Console.WriteLine(act)
						Console.WriteLine(vbLf)
					End If
				Next i
			Next j

			assertEquals(col6d, im2col)


			'Expected result:
			Dim expected As INDArray = Nd4j.create(miniBatch, depth, inH, inW)
			expected.put(New INDArrayIndex() {point(0), point(0), all(), all()},
			Nd4j.create(New Double()() {
				New Double() {0, 2, 4, 6, 4},
				New Double() {10, 24, 28, 32, 18},
				New Double() {20, 44, 48, 52, 28},
				New Double() {15, 32, 34, 36, 19}
			}))


			Dim col2imResult As INDArray = Nd4j.create(miniBatch, depth, inH, inW)
			Dim col2im As INDArray = Convolution.col2im(col6d, col2imResult, strideH, strideW, padTop, padLeft, inH, inW, dH, dW)

			assertEquals(expected, col2im)
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testConvOutWidthAndHeight(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testConvOutWidthAndHeight(ByVal backend As Nd4jBackend)
			Dim outSize As Long = Convolution.outSize(2, 1, 1, 2, 1, False)
			assertEquals(6, outSize)
		End Sub
	'
	'      @ParameterizedTest
	'    @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs")
	'    public void testIm2Col(Nd4jBackend backend) {
	'        INDArray linspaced = Nd4j.linspace(1, 16, 16, DataType.FLOAT).reshape(2, 2, 2, 2);
	'        INDArray ret = Convolution.im2col(linspaced, 1, 1, 1, 1, 2, 2, 0, false);
	'        System.out.println(ret);
	'    }
	'    



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled public void testCompareIm2ColImpl(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCompareIm2ColImpl(ByVal backend As Nd4jBackend)

			Dim miniBatches() As Integer = {1, 3, 5}
			Dim depths() As Integer = {1, 3, 5}
			Dim inHeights() As Integer = {5, 21}
			Dim inWidths() As Integer = {5, 21}
			Dim strideH() As Integer = {1, 2}
			Dim strideW() As Integer = {1, 2}
			Dim sizeW() As Integer = {1, 2, 3}
			Dim sizeH() As Integer = {1, 2, 3}
			Dim padH() As Integer = {0, 1, 2}
			Dim padW() As Integer = {0, 1, 2}
			Dim coverall() As Boolean = {False, True}

			Dim types() As DataType = {DataType.FLOAT, DataType.FLOAT, DataType.FLOAT, DataType.FLOAT}
			Dim modes() As DataBuffer.AllocationMode = {DataBuffer.AllocationMode.HEAP, DataBuffer.AllocationMode.HEAP, DataBuffer.AllocationMode.DIRECT, DataBuffer.AllocationMode.DIRECT}

			Dim factoryClassName As String = Nd4j.factory().GetType().ToString().ToLower()
			If factoryClassName.Contains("jcublas") OrElse factoryClassName.Contains("cuda") Then
				'Only test direct for CUDA; test all for CPU
				types = New DataType() {DataType.FLOAT, DataType.FLOAT}
				modes = New DataBuffer.AllocationMode() {DataBuffer.AllocationMode.DIRECT, DataBuffer.AllocationMode.DIRECT}
			End If

			Dim initialType As DataType = Nd4j.dataType()
			For i As Integer = 0 To types.Length - 1
				Dim type As DataType = types(i)
				Dim mode As DataBuffer.AllocationMode = modes(i)

				DataTypeUtil.setDTypeForContext(type)
				Nd4j.alloc = mode

				AllocUtil.setAllocationModeForContext(mode)

				For Each m As Integer In miniBatches
					For Each d As Integer In depths
						For Each h As Integer In inHeights
							For Each w As Integer In inWidths
								For Each sh As Integer In strideH
									For Each sw As Integer In strideW
										For Each kh As Integer In sizeH
											For Each kw As Integer In sizeW
												For Each ph As Integer In padH
													For Each pw As Integer In padW
														If (w - kw + 2 * pw) Mod sw <> 0 OrElse (h - kh + 2 * ph) Mod sh <> 0 Then
															Continue For '(w-kp+2*pW)/sw + 1 is not an integer,  i.e., number of outputs doesn't fit
														End If

														Console.WriteLine("Running " & m & " " & d & " " & h & " " & w)
														For Each [cAll] As Boolean In coverall

															Dim [in] As INDArray = Nd4j.rand(New Integer() {m, d, h, w})
															'assertEquals(in.data().allocationMode(), mode);
															'assertEquals(in.data().dataType(), opType);

															Dim outOrig As INDArray = OldConvolution.im2col([in], kh, kw, sh, sw, ph, pw, -1, [cAll]) 'Old implementation
															Dim outNew As INDArray = Convolution.im2col([in], kh, kw, sh, sw, ph, pw, [cAll]) 'Current implementation

															assertArrayEquals(outOrig.data().asFloat(), outNew.data().asFloat(), 0.01f)
															assertEquals(outOrig, outNew)
														Next [cAll]
													Next pw
												Next ph
											Next kw
										Next kh
									Next sw
								Next sh
							Next w
						Next h
					Next d
				Next m
			Next i

			DataTypeUtil.setDTypeForContext(initialType)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled public void testCompareIm2Col(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCompareIm2Col(ByVal backend As Nd4jBackend)

			Dim miniBatches() As Integer = {1, 3, 5}
			Dim depths() As Integer = {1, 3, 5}
			Dim inHeights() As Integer = {5, 21}
			Dim inWidths() As Integer = {5, 21}
			Dim strideH() As Integer = {1, 2}
			Dim strideW() As Integer = {1, 2}
			Dim sizeW() As Integer = {1, 2, 3}
			Dim sizeH() As Integer = {1, 2, 3}
			Dim padH() As Integer = {0, 1, 2}
			Dim padW() As Integer = {0, 1, 2}

			Dim types() As DataType = {DataType.FLOAT, DataType.FLOAT, DataType.FLOAT, DataType.FLOAT}
			Dim modes() As DataBuffer.AllocationMode = {DataBuffer.AllocationMode.HEAP, DataBuffer.AllocationMode.HEAP, DataBuffer.AllocationMode.DIRECT, DataBuffer.AllocationMode.DIRECT}

			Dim factoryClassName As String = Nd4j.factory().GetType().ToString().ToLower()
			If factoryClassName.Contains("jcublas") OrElse factoryClassName.Contains("cuda") Then
				'Only test direct for CUDA; test all for CPU
				types = New DataType() {DataType.FLOAT, DataType.FLOAT}
				modes = New DataBuffer.AllocationMode() {DataBuffer.AllocationMode.DIRECT, DataBuffer.AllocationMode.DIRECT}
			End If

			Dim inititalType As DataType = Nd4j.dataType()
			For i As Integer = 0 To types.Length - 1
				Dim type As DataType = types(i)
				Dim mode As DataBuffer.AllocationMode = modes(i)

				DataTypeUtil.setDTypeForContext(type)
				Nd4j.alloc = mode

				For Each m As Integer In miniBatches
					For Each d As Integer In depths
						For Each h As Integer In inHeights
							For Each w As Integer In inWidths
								For Each sh As Integer In strideH
									For Each sw As Integer In strideW
										For Each kh As Integer In sizeH
											For Each kw As Integer In sizeW
												For Each ph As Integer In padH
													For Each pw As Integer In padW
														Console.WriteLine("Before assertion")
														If (w - kw + 2 * pw) Mod sw <> 0 OrElse (h - kh + 2 * ph) Mod sh <> 0 Then
															Continue For '(w-kp+2*pW)/sw + 1 is not an integer, i.e., number of outputs doesn't fit
														End If

														Dim [in] As INDArray = Nd4j.rand(New Integer() {m, d, h, w})
														assertEquals([in].data().allocationMode(), mode)
														assertEquals([in].data().dataType(), type)
														Dim im2col As INDArray = Convolution.im2col([in], kh, kw, sh, sw, ph, pw, False) 'Cheating, to get correct shape for input

														Dim imgOutOld As INDArray = OldConvolution.col2im(im2col, sh, sw, ph, pw, h, w)
														Dim imgOutNew As INDArray = Convolution.col2im(im2col, sh, sw, ph, pw, h, w)
														Console.WriteLine("F order test")
														Console.WriteLine(imgOutOld)
														Console.WriteLine(imgOutNew)
														assertEquals(imgOutOld, imgOutNew)
													Next pw
												Next ph
											Next kw
										Next kh
									Next sw
								Next sh
							Next w
						Next h
					Next d
				Next m
			Next i

			DataTypeUtil.setDTypeForContext(inititalType)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCol2Im(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testCol2Im(ByVal backend As Nd4jBackend)
			Dim kh As Integer = 1
			Dim kw As Integer = 1
			Dim sy As Integer = 1
			Dim sx As Integer = 1
			Dim ph As Integer = 1
			Dim pw As Integer = 1
			Dim linspaced As INDArray = Nd4j.linspace(1, 64, 64, Nd4j.defaultFloatingPointType()).reshape(ChrW(2), 2, 2, 2, 2, 2)
			Dim newTest As INDArray = Convolution.col2im(linspaced, sy, sx, ph, pw, 2, 2)
			Dim assertion As INDArray = OldConvolution.col2im(linspaced, sy, sx, ph, pw, 2, 2)

			Console.WriteLine("Ordering of the result, new test: " & newTest.ordering())

			Console.WriteLine("Assertion dimensions: " & Arrays.toString(assertion.shape()))
			assertEquals(assertion, newTest)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testimcolim(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testimcolim(ByVal backend As Nd4jBackend)
			Dim nEx As Integer = 2
			Dim depth As Integer = 3
			Dim width As Integer = 7
			Dim height As Integer = 7
			Dim kernel() As Integer = {3, 2}
			Dim stride() As Integer = {2, 3}
			Dim padding() As Integer = {1, 2}
			Dim prod As Integer = nEx * depth * width * height

			Dim [in] As INDArray = Nd4j.linspace(1, prod, prod, Nd4j.defaultFloatingPointType()).reshape(ChrW(nEx), depth, width, height)

			Dim assertim2col As INDArray = OldConvolution.im2col([in], kernel, stride, padding)
			Dim im2col As INDArray = Convolution.im2col([in], kernel, stride, padding)
			assertEquals(assertim2col, im2col)

			Dim assertcol2im As INDArray = OldConvolution.col2im(im2col, stride, padding, height, width)
			Dim col2im As INDArray = Convolution.col2im(im2col, stride, padding, height, width)
			assertEquals(assertcol2im, col2im)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testIm2ColWithDilation(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testIm2ColWithDilation(ByVal backend As Nd4jBackend)
			Dim kH As Integer = 2
			Dim kW As Integer = 2
			Dim sH As Integer = 1
			Dim sW As Integer = 1
			Dim pH As Integer = 0
			Dim pW As Integer = 0
			Dim dH As Integer = 1
			Dim dW As Integer = 2
			Dim same As Boolean = False

	'        
	'        Input:
	'        [ 1,  2,  3
	'          4,  5,  6
	'          7,  8,  9 ]
	'
	'        Im2col:
	'        [ 1,  3
	'          4,  6 ]
	'
	'        [ 4,  6
	'          7,  9 ]
	'         


			Dim [in] As INDArray = Nd4j.create(DataType.DOUBLE, 1, 1, 3, 3)
			[in].get(point(0), point(0), all(), all()).assign(Nd4j.linspace(1, 9, 9, DataType.DOUBLE).reshape("c"c, 3, 3))

			Dim [out] As INDArray = Nd4j.create(DataType.DOUBLE, 1, 1, 2, 2, 2, 1) 'minibatch, depth, kH, kW, outH, outW
			Convolution.im2col([in], kH, kW, sH, sW, pH, pW, dH, dW, same, [out])

			Dim act0 As INDArray = [out].get(point(0), point(0), all(), all(), point(0), point(0))
			Dim act1 As INDArray = [out].get(point(0), point(0), all(), all(), point(1), point(0))

			Dim exp0 As INDArray = Nd4j.create(New Double()(){
				New Double() {1, 3},
				New Double() {4, 6}
			})
			Dim exp1 As INDArray = Nd4j.create(New Double()(){
				New Double() {4, 6},
				New Double() {7, 9}
			})

			assertEquals(exp0, act0)
			assertEquals(exp1, act1)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPoolingEdgeCases()
		Public Overridable Sub testPoolingEdgeCases()
			'Average pooling with same mode: should we include the padded values, when deciding what to divide by?
			'/*** Note: Mode 2 is the "DL4J always divide by kH*kW" approach ***

	'        
	'        Input:
	'        [ 1, 2, 3
	'          4, 5, 6
	'          7, 8, 9 ]
	'
	'
	'         Kernel 2, stride 1
	'         outH = 3, outW = 3 (i.e., ceil(in/stride)
	'         totalHPad = (outH-1) * strideH + kH - inH = (3-1)*1 + 2 - 3 = 1
	'         topPad = 0, bottomPad = 1
	'         leftPad = 0, rightPad = 1
	'         

			For Each inputOrder As Char In New Char(){"c"c, "f"c}
				For Each outputOrder As Char In New Char(){"c"c, "f"c}

					Dim input As INDArray = Nd4j.create(DataType.DOUBLE, 1, 1, 3, 3)
					input.get(point(0), point(0), all(), all()).assign(Nd4j.linspace(1, 9, 9, DataType.DOUBLE).reshape("c"c, 3, 3)).dup(inputOrder)

					input = input.dup("c"c)

					Dim input2 As INDArray = Nd4j.create(New Double(){1, 2, 3, 4, 5, 6, 7, 8, 9}, New Long(){1, 1, 3, 3}, "c"c) '.dup(inputOrder);
					assertEquals(input, input2)

					input = input2

					For i As Integer = 0 To 2
						For j As Integer = 0 To 2
							Console.Write(input.getDouble(0,0,i,j) & ",")
						Next j
						Console.WriteLine()
					Next i
					Console.WriteLine()

					Dim sums As INDArray = Nd4j.create(New Double()(){
						New Double() {(1 + 2 + 4 + 5), (2 + 3 + 5 + 6), (3 + 6)},
						New Double() {(4 + 5 + 7 + 8), (5 + 6 + 8 + 9), (6 + 9)},
						New Double() {(7 + 8), (8 + 9), (9)}
					})

					Dim divEnabled As INDArray = Nd4j.create(New Double()(){
						New Double() {4, 4, 2},
						New Double() {4, 4, 2},
						New Double() {2, 2, 1}
					})

					Dim expEnabled As INDArray = sums.div(divEnabled)
					Dim expDl4j As INDArray = sums.div(4)

					'https://github.com/deeplearning4j/libnd4j/blob/master/include/ops/declarable/generic/convo/pooling/avgpool2d.cpp
					Dim op1 As DynamicCustomOp = DynamicCustomOp.builder("avgpool2d").addIntegerArguments(2, 2, 1, 1, 0, 0, 1, 1, 1, 0, 0).addInputs(input).addOutputs(Nd4j.create(DataType.DOUBLE, New Long(){1, 1, 3, 3}, outputOrder)).build()

					Dim op2 As DynamicCustomOp = DynamicCustomOp.builder("avgpool2d").addIntegerArguments(2, 2, 1, 1, 0, 0, 1, 1, 1, 1, 0).addInputs(input).addOutputs(Nd4j.create(DataType.DOUBLE, New Long(){1, 1, 3, 3}, outputOrder)).build()

					Nd4j.Executioner.exec(op1)
					Nd4j.Executioner.exec(op2)
					Dim actEnabled As INDArray = op1.getOutputArgument(0)
					Dim actDl4j As INDArray = op2.getOutputArgument(0)


					Dim msg As String = "inOrder=" & inputOrder & ", outOrder=" & outputOrder
					Dim vr As val = actDl4j.get(point(0), point(0), all(), all())
					assertEquals(expDl4j, vr,msg)
					assertEquals(expEnabled, actEnabled.get(point(0), point(0), all(), all()),msg)
				Next outputOrder
			Next inputOrder
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPooling1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPooling1(ByVal backend As Nd4jBackend)
			For Each outputOrder As Char In New Char(){"c"c, "f"c}
				Dim exp As INDArray = Nd4j.create(New Single(){6.0f, 7.0f, 10.0f, 11.0f, 22.0f, 23.0f, 26.0f, 27.0f, 38.0f, 39.0f, 42.0f, 43.0f, 54.0f, 55.0f, 58.0f, 59.0f}, New Integer(){2, 2, 2, 2}, "c"c)

				Dim len As Integer = 2 * 4 * 4 * 2
				Dim x As INDArray = Nd4j.linspace(1, len, len, DataType.FLOAT).reshape("c"c, 2, 4, 4, 2)

				Dim op As DynamicCustomOp = DynamicCustomOp.builder("avgpool2d").addIntegerArguments(2, 2, 2, 2, 0, 0, 1, 1, 1, 1, 1).addInputs(x).addOutputs(Nd4j.create(DataType.FLOAT, New Long(){2, 2, 2, 2}, outputOrder)).build()

				Nd4j.Executioner.exec(op)

				Dim [out] As INDArray = op.getOutputArgument(0)

				assertEquals(exp, [out],"Output order: " & outputOrder)

	'            
	'            k=2, s=2, p=0, d=1, same mode, divisor = 1
	'
	'
	'            //c order: strides are descending... i.e., last dimension changes quickest
	'
	'            //Minibatch 0:
	'                //Depth 0
	'            [ 0,  1
	'              2,  3
	'              4,  5
	'              6,  7 ]
	'
	'                //Depth 1
	'             [ 8,  9
	'              10, 11
	'              12, 13
	'              14, 15 ]
	'
	'                //Depth 2
	'             [16, 17
	'              18, 19
	'              20, 21
	'              22, 23 ]
	'
	'                //Depth 3
	'             [24, 25
	'              26, 27
	'              28, 29
	'              30, 31 ]
	'
	'
	'
	'            //Minibatch 1:
	'
	'             


			Next outputOrder
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPooling2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPooling2(ByVal backend As Nd4jBackend)
			For Each outputOrder As Char In New Char(){"c"c, "f"c}
				Dim exp As INDArray = Nd4j.create(New Single(){6.0f, 7.0f, 10.0f, 11.0f, 22.0f, 23.0f, 26.0f, 27.0f, 38.0f, 39.0f, 42.0f, 43.0f, 54.0f, 55.0f, 58.0f, 59.0f}, New Integer(){2, 2, 2, 2}, "c"c)

				Dim len As Integer = 2 * 4 * 4 * 2
				Dim x As INDArray = Nd4j.linspace(1, len, len, DataType.FLOAT).reshape("c"c, 2, 4, 4, 2)

				Dim op As DynamicCustomOp = DynamicCustomOp.builder("avgpool2d").addIntegerArguments(2, 2, 2, 2, 0, 0, 1, 1, 0, 1, 1).addInputs(x).addOutputs(Nd4j.create(DataType.FLOAT,New Long(){2, 2, 2, 2}, outputOrder)).build()

				Nd4j.Executioner.exec(op)

				Dim [out] As INDArray = op.getOutputArgument(0)

				assertEquals(exp, [out],"Output order: " & outputOrder)
			Next outputOrder
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPooling3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPooling3(ByVal backend As Nd4jBackend)
			For Each outputOrder As Char In New Char(){"c"c, "f"c}
				Dim exp As INDArray = Nd4j.create(New Single(){11.0f, 12.0f, 15.0f, 16.0f, 27.0f, 28.0f, 31.0f, 32.0f, 43.0f, 44.0f, 47.0f, 48.0f, 59.0f, 60.0f, 63.0f, 64.0f}, New Integer(){2, 2, 2, 2}, "c"c)

				Dim len As Integer = 2 * 4 * 4 * 2
				Dim x As INDArray = Nd4j.linspace(1, len, len, DataType.FLOAT).reshape("c"c, 2, 4, 4, 2)

				Dim op As DynamicCustomOp = DynamicCustomOp.builder("maxpool2d").addIntegerArguments(2, 2, 2, 2, 0, 0, 1, 1, 1, 1, 1).addInputs(x).addOutputs(Nd4j.create(DataType.FLOAT, New Long(){2, 2, 2, 2}, outputOrder)).build()

				Nd4j.Executioner.exec(op)

				Dim [out] As INDArray = op.getOutputArgument(0)

				assertEquals(exp, [out],"Output order: " & outputOrder)
			Next outputOrder
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPooling4(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPooling4(ByVal backend As Nd4jBackend)
			For Each outputOrder As Char In New Char(){"c"c, "f"c}
				Dim exp As INDArray = Nd4j.create(New Single(){11.0f, 12.0f, 15.0f, 16.0f, 27.0f, 28.0f, 31.0f, 32.0f, 43.0f, 44.0f, 47.0f, 48.0f, 59.0f, 60.0f, 63.0f, 64.0f}, New Integer(){2, 2, 2, 2}, "c"c)

				Dim len As Integer = 2 * 4 * 4 * 2
				Dim x As INDArray = Nd4j.linspace(1, len, len, DataType.FLOAT).reshape("c"c, 2, 4, 4, 2)

				Dim op As DynamicCustomOp = DynamicCustomOp.builder("maxpool2d").addIntegerArguments(2, 2, 2, 2, 0, 0, 1, 1, 0, 1, 1).addInputs(x).addOutputs(Nd4j.create(DataType.FLOAT, New Long(){2, 2, 2, 2}, outputOrder)).build()

				Nd4j.Executioner.exec(op)

				Dim [out] As INDArray = op.getOutputArgument(0)

				assertEquals(exp, [out],"Output order: " & outputOrder)
			Next outputOrder
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPooling5(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPooling5(ByVal backend As Nd4jBackend)
			For Each outputOrder As Char In New Char(){"c"c, "f"c}
				Dim exp As INDArray = Nd4j.create(New Single(){7.0f, 8.0f, 11.0f, 12.0f, 14.0f, 15.0f, 27.0f, 28.0f, 31.0f, 32.0f, 34.0f, 35.0f, 42.0f, 43.0f, 46.0f, 47.0f, 49.0f, 50.0f, 57.0f, 58.0f, 61.0f, 62.0f, 64.0f, 65.0f, 77.0f, 78.0f, 81.0f, 82.0f, 84.0f, 85.0f, 92.0f, 93.0f, 96.0f, 97.0f, 99.0f, 100.0f}, New Integer(){2, 3, 3, 2}, "c"c)

				Dim len As Integer = 2 * 5 * 5 * 2
				Dim x As INDArray = Nd4j.linspace(1, len, len, DataType.FLOAT).reshape("c"c, 2, 5, 5, 2)

				Dim op As DynamicCustomOp = DynamicCustomOp.builder("avgpool2d").addIntegerArguments(2, 2, 2, 2, 0, 0, 1, 1, 1, 0, 1).addInputs(x).addOutputs(Nd4j.create(DataType.FLOAT, New Long(){2, 3, 3, 2}, outputOrder)).build()

				Nd4j.Executioner.exec(op)

				Dim [out] As INDArray = op.getOutputArgument(0)

				assertEquals(exp, [out],"Output order: " & outputOrder)
			Next outputOrder
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPooling6(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPooling6(ByVal backend As Nd4jBackend)
			For Each outputOrder As Char In New Char(){"c"c, "f"c}
				Dim exp As INDArray = Nd4j.create(New Single(){7.0f, 8.0f, 11.0f, 12.0f, 27.0f, 28.0f, 31.0f, 32.0f, 57.0f, 58.0f, 61.0f, 62.0f, 77.0f, 78.0f, 81.0f, 82.0f}, New Integer(){2, 2, 2, 2}, "c"c)

				Dim len As Integer = 2 * 5 * 5 * 2
				Dim x As INDArray = Nd4j.linspace(1, len, len, DataType.FLOAT).reshape("c"c, 2, 5, 5, 2)

				Dim op As DynamicCustomOp = DynamicCustomOp.builder("avgpool2d").addIntegerArguments(2, 2, 2, 2, 0, 0, 1, 1, 0, 1, 1).addInputs(x).addOutputs(Nd4j.create(DataType.FLOAT, New Long(){2, 2, 2, 2}, outputOrder)).build()

				Nd4j.Executioner.exec(op)

				Dim [out] As INDArray = op.getOutputArgument(0)

				assertEquals(exp, [out],"Output order: " & outputOrder)
			Next outputOrder
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPooling7(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPooling7(ByVal backend As Nd4jBackend)
			For Each outputOrder As Char In New Char(){"c"c, "f"c}
				Dim exp As INDArray = Nd4j.create(New Single(){7.0f, 9.0f, 17.0f, 19.0f, 32.0f, 34.0f, 42.0f, 44.0f, 57.0f, 59.0f, 67.0f, 69.0f, 82.0f, 84.0f, 92.0f, 94.0f}, New Integer(){2, 2, 2, 2}, "c"c)

				Dim len As Integer = 2 * 2 * 5 * 5
				Dim x As INDArray = Nd4j.linspace(1, len, len, DataType.FLOAT).reshape("c"c, 2, 2, 5, 5)

				Dim op As DynamicCustomOp = DynamicCustomOp.builder("maxpool2d").addIntegerArguments(2, 2, 2, 2, 0, 0, 1, 1, 0, 1, 0).addInputs(x).addOutputs(Nd4j.create(DataType.FLOAT, New Long(){2, 2, 2, 2}, outputOrder)).build()

				Nd4j.Executioner.exec(op)

				Dim [out] As INDArray = op.getOutputArgument(0)

				assertEquals(exp, [out],"Output order: " & outputOrder)
			Next outputOrder
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPooling8(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPooling8(ByVal backend As Nd4jBackend)
			For Each outputOrder As Char In New Char(){"c"c, "f"c}
				Dim exp As INDArray = Nd4j.create(New Single(){1.0f, 2.5f, 4.5f, 8.5f, 10.0f, 12.0f, 18.5f, 20.0f, 22.0f, 26.0f, 27.5f, 29.5f, 33.5f, 35.0f, 37.0f, 43.5f, 45.0f, 47.0f, 51.0f, 52.5f, 54.5f, 58.5f, 60.0f, 62.0f, 68.5f, 70.0f, 72.0f, 76.0f, 77.5f, 79.5f, 83.5f, 85.0f, 87.0f, 93.5f, 95.0f, 97.0f}, New Integer(){2, 2, 3, 3}, "c"c)

				Dim len As Integer = 2 * 2 * 5 * 5
				Dim x As INDArray = Nd4j.linspace(1, len, len, DataType.FLOAT).reshape("c"c, 2, 2, 5, 5)

				Dim op As DynamicCustomOp = DynamicCustomOp.builder("avgpool2d").addIntegerArguments(2, 2, 2, 2, 1, 1, 1, 1, 0, 0, 0).addInputs(x).addOutputs(Nd4j.create(DataType.FLOAT, New Long(){2, 2, 3, 3}, outputOrder)).build()

				Nd4j.Executioner.exec(op)

				Dim [out] As INDArray = op.getOutputArgument(0)

				assertEquals(exp, [out],"Output order: " & outputOrder)
			Next outputOrder
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPooling9(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPooling9(ByVal backend As Nd4jBackend)
			For Each outputOrder As Char In New Char(){"c"c, "f"c}
				Dim exp As INDArray = Nd4j.create(New Single(){0.25f, 1.25f, 2.25f, 4.25f, 10.0f, 12.0f, 9.25f, 20.0f, 22.0f, 6.5f, 13.75f, 14.75f, 16.75f, 35.0f, 37.0f, 21.75f, 45.0f, 47.0f, 12.75f, 26.25f, 27.25f, 29.25f, 60.0f, 62.0f, 34.25f, 70.0f, 72.0f, 19.0f, 38.75f, 39.75f, 41.75f, 85.0f, 87.0f, 46.75f, 95.0f, 97.0f}, New Integer(){2, 2, 3, 3}, "c"c)

				Dim len As Integer = 2 * 2 * 5 * 5
				Dim x As INDArray = Nd4j.linspace(1, len, len, DataType.FLOAT).reshape("c"c, 2, 2, 5, 5)

				Dim op As DynamicCustomOp = DynamicCustomOp.builder("avgpool2d").addIntegerArguments(2, 2, 2, 2, 1, 1, 1, 1, 0, 1, 0).addInputs(x).addOutputs(Nd4j.create(DataType.FLOAT, New Long(){2, 2, 3, 3}, outputOrder)).build()

				Nd4j.Executioner.exec(op)

				Dim [out] As INDArray = op.getOutputArgument(0)

				assertEquals(exp, [out],"Output order: " & outputOrder)
			Next outputOrder
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPooling10(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPooling10(ByVal backend As Nd4jBackend)
			For Each outputOrder As Char In New Char(){"c"c, "f"c}
				Dim exp As INDArray = Nd4j.create(New Single(){4.0f, 6.0f, 7.5f, 14.0f, 16.0f, 17.5f, 21.5f, 23.5f, 25.0f, 29.0f, 31.0f, 32.5f, 39.0f, 41.0f, 42.5f, 46.5f, 48.5f, 50.0f, 54.0f, 56.0f, 57.5f, 64.0f, 66.0f, 67.5f, 71.5f, 73.5f, 75.0f, 79.0f, 81.0f, 82.5f, 89.0f, 91.0f, 92.5f, 96.5f, 98.5f, 100.0f}, New Integer(){2, 2, 3, 3}, "c"c)

				Dim len As Integer = 2 * 2 * 5 * 5
				Dim x As INDArray = Nd4j.linspace(1, len, len, DataType.FLOAT).reshape("c"c, 2, 2, 5, 5)

				Dim op As DynamicCustomOp = DynamicCustomOp.builder("avgpool2d").addIntegerArguments(2, 2, 2, 2, 0, 0, 1, 1, 1, 0, 0).addInputs(x).addOutputs(Nd4j.create(DataType.FLOAT, New Long(){2, 2, 3, 3}, outputOrder)).build()

				Nd4j.Executioner.exec(op)

				Dim [out] As INDArray = op.getOutputArgument(0)

				assertEquals(exp, [out],"Output order: " & outputOrder)
			Next outputOrder
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPooling11(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPooling11(ByVal backend As Nd4jBackend)
			For Each outputOrder As Char In New Char(){"c"c, "f"c}
				Dim exp As INDArray = Nd4j.create(New Single(){3, 4, 6, 7}, New Integer(){1, 1, 2, 2}, "c"c)

				Dim len As Integer = 1 * 1 * 3 * 3
				Dim x As INDArray = Nd4j.linspace(1, len, len, DataType.FLOAT).reshape("c"c, 1, 1, 3, 3)

				Dim op As DynamicCustomOp = DynamicCustomOp.builder("avgpool2d").addIntegerArguments(2, 2, 1, 1, 0, 0, 1, 1, 0, 0, 0).addInputs(x).addOutputs(Nd4j.create(DataType.FLOAT, New Long(){1, 1, 2, 2}, outputOrder)).build()

				Nd4j.Executioner.exec(op)

				Dim [out] As INDArray = op.getOutputArgument(0)

				assertEquals(exp, [out],"Output order: " & outputOrder)
			Next outputOrder
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPooling12(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPooling12(ByVal backend As Nd4jBackend)
			For Each outputOrder As Char In New Char(){"c"c, "f"c}
				Dim exp As INDArray = Nd4j.create(New Single(){3.0f, 4.0f, 4.5f, 6.0f, 7.0f, 7.5f, 7.5f, 8.5f, 9.0f}, New Integer(){1, 1, 3, 3}, "c"c)

				Dim len As Integer = 1 * 1 * 3 * 3
				Dim x As INDArray = Nd4j.linspace(1, len, len, DataType.FLOAT).reshape("c"c, 1, 1, 3, 3)

				Dim op As DynamicCustomOp = DynamicCustomOp.builder("avgpool2d").addIntegerArguments(2, 2, 1, 1, 0, 0, 1, 1, 1, 0, 0).addInputs(x).addOutputs(Nd4j.create(DataType.FLOAT, New Long(){1, 1, 3, 3}, outputOrder)).build()

				Nd4j.Executioner.exec(op)

				Dim [out] As INDArray = op.getOutputArgument(0)

				assertEquals(exp, [out],"Output order: " & outputOrder)
			Next outputOrder
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPooling13(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPooling13(ByVal backend As Nd4jBackend)
			For Each outputOrder As Char In New Char(){"c"c}
				Dim exp As INDArray = Nd4j.create(New Single(){3.0f, 4.0f, 4.5f, 6.0f, 7.0f, 7.5f, 7.5f, 8.5f, 9.0f}, New Integer(){1, 1, 3, 3}, "c"c)

				Dim len As Integer = 1 * 1 * 3 * 3
				Dim x As INDArray = Nd4j.linspace(1, len, len, DataType.FLOAT).reshape("c"c, 1, 1, 3, 3)

				Dim op As DynamicCustomOp = DynamicCustomOp.builder("avgpool2d").addIntegerArguments(2, 2, 1, 1, 0, 0, 1, 1, 1, 0, 0).addInputs(x).addOutputs(Nd4j.create(DataType.FLOAT, New Long(){1, 1, 3, 3}, outputOrder)).build()

				Nd4j.Executioner.exec(op)

				Dim [out] As INDArray = op.getOutputArgument(0)

				assertEquals(exp, [out],"Output order: " & outputOrder)
			Next outputOrder
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPoolingDilation()
		Public Overridable Sub testPoolingDilation()

			Dim inputShape() As Integer = {1, 1, 4, 5}
			Dim outH As Integer = inputShape(2)
			Dim outW As Integer = inputShape(3)

			Dim kernel() As Integer = {2, 2}
			Dim strides() As Integer = {1, 1}
			Dim pad() As Integer = {1, 1} 'From same mode
			Dim dilation() As Integer = {2, 2}
			Dim same As Boolean = True

	'        
	'        Input:
	'        [ 1,  2,  3,  4,  5
	'          6,  7,  8,  9, 10
	'         11, 12, 13, 14, 15
	'         16, 17, 18, 19, 20 ]
	'
	'        Input with SAME padding:
	'        [ 0,  0,  0,  0,  0,  0,  0
	'          0,  1,  2,  3,  4,  5,  0
	'          0,  6,  7,  8,  9, 10,  0
	'          0, 11, 12, 13, 14, 15,  0
	'          0, 16, 17, 18, 19, 20,  0
	'          0,  0,  0,  0,  0,  0,  0]
	'
	'         4x5 in
	'         Same mode, stride 1, dilation 2, kernel 2
	'         kHEffective = (2 + (2-1)*(2-1)) = 3
	'         oH = ceil(iH/sH) = 4
	'         oW = ceil(iW/sW) = 5
	'         totalPadH = (oH-1)*sH + kH - inH = (4-1)*1 + 3 - 4 = 2
	'         padTop = 1, padBottom = 1
	'
	'         totalPadW = (oW-1)*sW + kW - inW = (5-1)*1 + 3 - 5 = 2
	'         padLeft = 1, padRight = 1
	'
	'        [ 0,  0]    [ 0,  0]    [ 0,  0]    [ 0,  0]    [ 0,  0]
	'        [ 0,  7]    [ 6,  8]    [ 7,  9]    [ 8, 10]    [ 9,  0]
	'
	'        [ 0   2]    [ 1,  3]    [ 2,  4]    [ 3,  5]    [ 4,  0]
	'        [ 0, 12]    [11, 13]    [12, 14]    [13, 15]    [14,  0]
	'
	'        [ 0,  7]    [ 6,  8]    [ 7,  9]    [ 8, 10]    [ 9,  0]
	'        [ 0, 17]    [16, 18]    [17, 19]    [18, 20]    [19,  0]
	'
	'        [ 0, 12]    [11, 13]    [12, 14]    [13, 15]    [14,  0]
	'        [ 0,  0],   [ 0,  0]    [ 0,  0]    [ 0,  0]    [ 0,  0]
	'         

			Dim origInput As INDArray = Nd4j.create(DataType.DOUBLE, ArrayUtil.toLongArray(inputShape))
			origInput.get(point(0), point(0), all(), all()).assign(Nd4j.linspace(1,20,20, DataType.DOUBLE).reshape("c"c,4,5))


			Dim expMax As INDArray = Nd4j.create(DataType.DOUBLE, 1,1,4,5)
			expMax.get(point(0), point(0), all(), all()).assign(Nd4j.create(New Double()(){
				New Double() { 7, 8, 9, 10, 9},
				New Double() {12, 13, 14, 15, 14},
				New Double() {17, 18, 19, 20, 19},
				New Double() {12, 13, 14, 15, 14}
			}))

			Dim sum As INDArray = Nd4j.create(DataType.DOUBLE, 1,1,4,5)
			sum.get(point(0), point(0), all(), all()).assign(Nd4j.create(New Double()(){
				New Double() { 7, (6+8), (7+9), (8+10), 9},
				New Double() {(2+12), (1+3+11+13), (2+4+12+14), (3+5+13+15), (4+14)},
				New Double() {(7+17), (6+8+16+18), (7+9+17+19), (8+10+18+20), (9+19)},
				New Double() {12, (11+13), (12+14), (13+15), 14}
			}))
			Dim expAvgExclude As INDArray = sum.dup()
			expAvgExclude.get(point(0), point(0), all(), all()).divi(Nd4j.create(New Double()(){
				New Double() { 1, 2, 2, 2, 1},
				New Double() { 2, 4, 4, 4, 2},
				New Double() { 2, 4, 4, 4, 2},
				New Double() { 1, 2, 2, 2, 1}
			}))

			Dim expAvgInclude As INDArray = sum.div(4.0)


			Dim testNum As Integer = 0
			For i As Integer = 0 To 2


				Dim inputs As IList(Of Pair(Of INDArray, String)) = NDArrayCreationUtil.getAll4dTestArraysWithShape(12345, inputShape, DataType.DOUBLE)

				For Each pIn As Pair(Of INDArray, String) In inputs
					Dim input As INDArray = pIn.First.assign(origInput)

					Dim [out] As INDArray = Nd4j.create(DataType.DOUBLE, input.shape(), "c"c)

					'TODO Test on weird strides also (i.e., remove the dup here)
					input = input.dup("c"c)

					Dim exp As INDArray
					Dim mode As String
					Select Case i
						Case 0 'Max
							Convolution.pooling2D(input, kernel(0), kernel(1), strides(0), strides(1), pad(0), pad(1), dilation(0), dilation(1), same, Pooling2D.Pooling2DType.MAX, Pooling2D.Divisor.INCLUDE_PADDING, 0.0, outH, outW, [out])
							exp = expMax
							mode = "max"
						Case 1 'Avg + mode 0 (exclude padding)
							Convolution.pooling2D(input, kernel(0), kernel(1), strides(0), strides(1), pad(0), pad(1), dilation(0), dilation(1), same, Pooling2D.Pooling2DType.AVG, Pooling2D.Divisor.EXCLUDE_PADDING, 0.0, outH, outW, [out])
							exp = expAvgExclude
							mode = "avg_0"
						Case 2 'Avg + mode 1 (include padding)
							Convolution.pooling2D(input, kernel(0), kernel(1), strides(0), strides(1), pad(0), pad(1), dilation(0), dilation(1), same, Pooling2D.Pooling2DType.AVG, Pooling2D.Divisor.INCLUDE_PADDING, 0.0, outH, outW, [out])
							exp = expAvgInclude
							mode = "avg_2"
						Case Else
							Throw New Exception()
					End Select

					Dim msg As String = "TestNum=" & testNum & ", Mode: " & mode & ", " & pIn.Second
					assertEquals(exp, [out],msg)
					testNum += 1
				Next pIn
			Next i
		End Sub


		Public Overrides Function ordering() As Char
			Return "f"c
		End Function
	End Class

End Namespace